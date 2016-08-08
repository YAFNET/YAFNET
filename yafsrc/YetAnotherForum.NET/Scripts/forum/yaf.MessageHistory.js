function toggleNewSelection(source) {
    var isChecked = source.checked;
    $("input[id*='New']").each(function () {
        $(this).attr('checked', false);
    });
    source.checked = isChecked;
}

function toggleOldSelection(source) {
    var isChecked = source.checked;
    $("input[id*='Old']").each(function () {
        $(this).attr('checked', false);
    });
    source.checked = isChecked;
}


/// <summary>
/// Renders the message difference.
/// </summary>
/// <param name="messageEditedAtText">The message edited at text.</param>
/// <param name="nothingSelectedText">The nothing selected text.</param>
/// <param name="selectBothText">The select both text.</param>
/// <param name="selectDifferentText">The select different text.</param>
/// <returns></returns>
function RenderMessageDiff(messageEditedAtText, nothingSelectedText, selectBothText, selectDifferentText) {
    var oldElement = $("input[id*='New']:checked");
    var newElement = $("input[id*='Old']:checked");

    if (newElement.length && oldElement.length) {
        // check if two different messages are selected
        if ($("input[id*='Old']:checked").attr('id').slice(-1) == $("input[id*='New']:checked").attr('id').slice(-1)) {
            alert(selectDifferentText);
        } else {
            var base = difflib.stringAsLines($("input[id*='Old']:checked").parent().next().next().find("input[id*='MessageField']").attr('value'));
            var newtxt = difflib.stringAsLines($("input[id*='New']:checked").parent().next().find("input[id*='MessageField']").attr('value'));
            var sm = new difflib.SequenceMatcher(base, newtxt);
            var opcodes = sm.get_opcodes();

            $("#diffContent").html('<div class="diffContent">' + diffview.buildView({
                baseTextLines: base,
                newTextLines: newtxt,
                opcodes: opcodes,
                baseTextName: messageEditedAtText + oldElement.parent().next().next().next().next().html(),
                newTextName: messageEditedAtText + oldElement.parent().next().next().next().next().html(),
                contextSize: 3,
                viewType: 0
            }).outerHTML + '</div>');
        }
    }
    else if (newElement.length || oldElement.length) {
        alert(selectBothText);
    } else {
        alert(nothingSelectedText);
    }
}