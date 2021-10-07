document.getElementById("TextArea").addEventListener(
    "input", function ()
    {
        document.getElementById("NoteBlock").innerHTML = this.value;
    }
);