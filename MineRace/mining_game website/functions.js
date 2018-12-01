window.addEventListener("DOMContentLoaded", function() {
    var title = document.getElementById("title_text");
    title.onclick = function() {
        if (title.innerHTML == "GG") {
            title.innerHTML = "MINING GAME";
        } else {
            title.innerHTML = "GG"
        }
    }
}, false);