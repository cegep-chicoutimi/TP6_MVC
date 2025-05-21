var showWarningMessage = function (message) {
    $(".warning span").text(message);
    $(".warning").show("slow");
};

var hideWarningMessage = function () {
    $(".warning span").text("");
    $(".warning").hide();
};

var onColorClick = function (el) {
    hideWarningMessage();

    var selectedColorIndex = $("#selected-color-index").val();
    var newColorIndex = $(el).attr("data-color-index");

    if (selectedColorIndex != newColorIndex) {
        if (selectedColorIndex != "") {
            $(".colors .color-" + selectedColorIndex).css({ "height": "25px", "width": "25px" });
        }

        $("#selected-color-index").val(newColorIndex);
        $(el).css({ "height": "30px", "width": "30px" });
    }
};

var onPositionClick = function (el) {
    hideWarningMessage();

    var selectedColorIndex = $("#selected-color-index").val();

    if (selectedColorIndex != "") {
        var currentColorClass = $(el).attr("data-color-class");
        var newColorClass = $(".colors .color-" + selectedColorIndex).attr("class");

        if (currentColorClass != newColorClass) {
            var targetPositionIndex = $(el).attr("data-position-index");

            if (currentColorClass != "") {
                $(el).removeClass(currentColorClass);
            }

            $(el).addClass(newColorClass + " no-border");
            $(el).attr("data-color-class", newColorClass);
            $("#color-index-" + targetPositionIndex).val(selectedColorIndex);
        }
    } else {
        showWarningMessage("Vous devez sélectionner une couleur");
    }
};

var allPositionsAreFilled = function () {
    var nbFilled = 0;
    var nbPositions = $("#nb-positions").val();

    for (var position = 1; position <= nbPositions; position++) {
        var colorIndex = $("#color-index-" + position).val();
        if (colorIndex > 0) {
            nbFilled++;
        }
    }

    return nbFilled == nbPositions;
}

var validateChoices = function (el) {
    if (allPositionsAreFilled()) {
        $.ajax({
            url: el.action,
            type: 'POST',
            data: $(el).serialize()
        }).done(function (result) {
            $("#game-container").html(result);
        });
    } else {
        showWarningMessage("Vous devez remplir toutes les positions");
    }
};
