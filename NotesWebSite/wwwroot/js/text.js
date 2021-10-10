var TextArea = document.getElementById("TextArea");
var NoteBlock = document.getElementById("NoteBlock");

function fillText() {
    NoteBlock.innerHTML = TextArea.value;
}

fillText();
TextArea.addEventListener("input", fillText);