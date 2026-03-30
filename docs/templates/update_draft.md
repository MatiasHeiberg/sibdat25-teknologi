<%*
// 1. Find anker-filen uanset hvad den hedder, eller hvor den ligger.
// Vi leder i metadata-cachen efter den fil, der har 'is_draft: true'.
const allFiles = app.vault.getMarkdownFiles();
let draftFile = null;
let draftCache = null;

for (let file of allFiles) {
    const cache = app.metadataCache.getFileCache(file);
    if (cache?.frontmatter?.is_draft === true) {
        draftFile = file;
        draftCache = cache;
        break; // Vi fandt den! Stop med at lede.
    }
}

// Sikkerhedsnet hvis anker-filen blev slettet
if (!draftFile) {
    new Notice("Kunne ikke finde Draft-noten! Husk at tilføje 'is_draft: true' i YAML.");
    return "";
}

// 2. Læs filens nuværende indhold og beskyt YAML-boksen
const fileContent = await app.vault.read(draftFile);
const yamlMatch = fileContent.match(/^---\n[\s\S]*?\n---/);
const yamlBlock = yamlMatch ? yamlMatch[0] : ""; // Gemmer den præcise YAML-tekst

// 3. Hent sektionerne fra anker-filens YAML
const sectionOrder = draftCache?.frontmatter?.sections || [];

if (sectionOrder.length === 0) {
    new Notice("Advarsel: Ingen 'sections' fundet i toppen af " + draftFile.basename);
} else {
    new Notice("Bygger rapport ud fra " + sectionOrder.length + " sektioner...");
}

// 4. Hent alle "atomic notes" uanset hvor de ligger i systemet.
// Vi filtrerer filer fra, der slet ikke er en del af Bases-systemet.
let notesPerSection = {};

for (let file of allFiles) {
    if (file.path === draftFile.path) continue; // Spring selve Draft-noten over

    const noteCache = app.metadataCache.getFileCache(file);

    // Ignorer filer, der er markeret med 'exclude: true'
    if (noteCache?.frontmatter?.exclude === true) continue;

    const section = noteCache?.frontmatter?.section;

    // Hvis noten ikke har en section property, skal den slet ikke inkluderes.
    if (!section || typeof section !== 'string' || section.trim().length === 0) continue;

    const sortKey = parseFloat(noteCache?.frontmatter?.sortKey);
    const safeSortKey = isNaN(sortKey) ? 999 : sortKey;
    const noteObj = { name: file.basename, sortKey: safeSortKey, section: section };

    // Sorter i den rigtige spand
    if (!notesPerSection[section]) notesPerSection[section] = [];
    notesPerSection[section].push(noteObj);
}

// 5. Byg den nye tekst (start med at indsætte den fredede YAML-boks)
let newText = yamlBlock + "\n\n";

// Filtrer sectionOrder for sektioner der faktisk har noter (fjerner ubrugte gamle overskrifter)
const activeOrderedSections = sectionOrder.filter(sec => notesPerSection[sec] && notesPerSection[sec].length > 0);

// Find sektioner der findes i noterne, men ikke i sectionOrder (nye sektioner)
const newSections = Object.keys(notesPerSection).filter(sec => !sectionOrder.includes(sec));
newSections.sort(); // Sorter de nye sektioner alfabetisk

// Saml den endelige liste: Først de kendte (sorteret efter ønsket rækkefølge), så de nye
const finalSectionList = [...activeOrderedSections, ...newSections];

for (let currentSection of finalSectionList) {
    newText += `## ${currentSection}\n\n`;
    let notes = notesPerSection[currentSection] || [];
    
    // Sorter noterne i den specifikke sektion
    notes.sort((a, b) => a.sortKey - b.sortKey);

    for (let note of notes) {
        newText += `![[${note.name}]]\n\n`;
    }
}

// 7. Overskriv filen i baggrunden!
await app.vault.modify(draftFile, newText);
new Notice("Draft opdateret succesfuldt!", 3000);

// Returner en tom streng, så Templater ikke sætter noget ind der hvor markøren er
return "";
%>