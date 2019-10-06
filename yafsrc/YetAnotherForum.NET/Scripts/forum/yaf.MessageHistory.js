function toggleSelection(source) {
    if ($("input[id*='Compare']:checked").length > 2) {
        source.checked = false;
        bootbox.alert("Only 2 Versions can be selected vor comparing!");
    }
}

function RenderMessageDiff(messageEditedAtText, nothingSelectedText, selectBothText, selectDifferentText) {
    var oldElement = $("input[id*='Compare']:checked").first();
    var newElement = $("input[id*='Compare']:checked").eq(1);

    if (newElement.length && oldElement.length) {
        var oldText = difflib.stringAsLines(oldElement.parent().next().attr("value"));
        var newText = difflib.stringAsLines(newElement.parent().next().attr("value"));
        var sm = new difflib.SequenceMatcher(oldText, newText);
        var opCodes = sm.get_opcodes();

        $("#diffContent").html('<div class="diffContent">' + diffview.buildView({
            baseTextLines: oldText,
            newTextLines: newText,
            opcodes: opCodes,
            baseTextName: oldElement.parent().parent().next()[0].outerText,
            newTextName: newElement.parent().parent().next()[0].outerText,
            contextSize: 3,
            viewType: 1
        }).outerHTML + "</div>");
    }
    else if (newElement.length || oldElement.length) {
        bootbox.alert(selectBothText);
    } else {
        bootbox.alert(nothingSelectedText);
    }
}