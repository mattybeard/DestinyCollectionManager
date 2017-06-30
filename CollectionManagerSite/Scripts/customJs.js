
function processNeededResults(neededArray, sourceDiv) {
    $('#' + sourceDiv).empty();
    if (neededArray.length > 0) {
        var factionDiv = "<div class=\"prose\"><h2><span>Still Missing</span></h2></div>";
        $.each(neededArray, function(index, faction) {
            factionDiv += "<div class=\"items js-toggle\">";
            factionDiv += "<div class=\"prose\">";
            factionDiv += "<h3 class=\"items__control js-toggle-control is-active\">";
            factionDiv += faction.FactionName;
            factionDiv += "</h3>";
            factionDiv += "</div>";
            factionDiv += "<div class=\"grid items__content js-toggle-content\">";
            $.each(faction.Items, function(index, item) {
                factionDiv += "<div class=\"grid__item fablet-one-half tablet-one-third\">";
                factionDiv += "<div class=\"item\">";
                factionDiv += "<div class=\"grid grid--middled\">";
                factionDiv += "<div class=\"grid__item auto\">";
                factionDiv += "<div class=\"item__image\">";
                factionDiv += "<img src=\"https://www.bungie.net" +
                    item.Icon +
                    "\" alt=\"\" />";
                factionDiv += " </div>";
                factionDiv += " </div>";
                factionDiv += "<div class=\"grid__item one-half\">";
                factionDiv += "<div class=\"item__title\">";
                factionDiv += item.Name;
                factionDiv += " </div>";
                factionDiv += " </div>";
                factionDiv += " </div>";
                factionDiv += " </div>";
                factionDiv += " </div>";
            });

            factionDiv += "</div>";
            factionDiv += "</div>";
        });
        $('#' + sourceDiv).append(factionDiv);
    }
}

function processForSaleResults(forsaleArray, sourceDiv) {

    $('#' + sourceDiv).empty();

    if (forsaleArray.length > 0) {
        var neededItems = "<div class=\"prose\"><h2><span>Currently For sale</span></h2></div>";
        $.each(forsaleArray, function (index, faction) {
            neededItems += "<div class=\"items\">";
            neededItems += "<div class=\"prose\">";
            neededItems += "<h3 class=\"items__control js-toggle-control\">" +
                faction.FactionName +
                "</h3>";
            neededItems += "</div>";
            neededItems += "<div class=\"grid items__content js-toggle-content\">";
            $.each(faction.Items, function (index, item) {
                neededItems += "<div class=\"grid__item fablet-one-half tablet-one-third\">";
                neededItems += "<div class=\"item\">";
                neededItems += "<div class=\"grid grid--middled\">";
                neededItems += "<div class=\"grid__item auto\">";
                neededItems += "<div class=\"item__image\">";
                neededItems += "<img src=\"https://www.bungie.net" + item.Icon + "\" alt=\"\" />";
                neededItems += "</div>";
                neededItems += " </div>";
                neededItems += "<div class=\"grid__item one-half\"><div class=\"item__title\">" + item.Name + "</div></div>";
                neededItems += " </div>";
                neededItems += " </div>";
                neededItems += " </div>";
            });
            neededItems += "</div>";
            neededItems += "</div>";
        });

        $('#' + sourceDiv).append(neededItems);
    }
}

function ProcessResults(data) {
    var obj = JSON.parse(data);

    var expiryDate = "<a id=\"emblems\" class=\"js-tab-heading\">Emblems</a><span data-date=\"2017-06-27T08:00:00Z\" role=\"timer\" class=\"countdown js-countdown\"><span class=\"countdown__title\">Next reset</span><span class=\"countdown__units js-countdown-units\">27 Jun 2017</span></span>";
    $('#emblemH1').html(expiryDate);

    if (obj.Emblems.ForSale.length > 0) {
        $('#emblemsForSale').empty();
        var neededEmblems = "<div class=\"prose\"><h2><span>Currently For sale</span></h2></div>";
        $.each(obj.Emblems.ForSale, function (index, faction) {
            neededEmblems += "<div class=\"items\">";
            neededEmblems += "<div class=\"prose\">";
            neededEmblems += "<h3 class=\"items__control js-toggle-control\">" +
                faction.FactionName +
                "</h3>";
            neededEmblems += "</div>";
            neededEmblems += "<div class=\"grid items__content js-toggle-content\">";
            $.each(faction.Items, function (index, item) {
                neededEmblems += "<div class=\"grid__item fablet-one-half tablet-one-third\">";
                neededEmblems += "<div style=\"background-image: url(https://www.bungie.net" + item.SecondaryIcon + ")\" class=\"item\">";
                neededEmblems += "<div class=\"grid grid--middled\">";
                neededEmblems += "<div class=\"grid__item auto\">";
                neededEmblems += "<div class=\"item__image\">";
                neededEmblems += "<img src=\"https://www.bungie.net" + item.Icon + "\" alt=\"\" />";
                neededEmblems += "</div>";
                neededEmblems += " </div>";
                neededEmblems += "<div class=\"grid__item one-half\"><div class=\"item__title\">" + item.Name + "</div></div>";
                neededEmblems += " </div>";
                neededEmblems += " </div>";
                neededEmblems += " </div>";
            });
            neededEmblems += "</div>";
            neededEmblems += "</div>";
        });

        $('#emblemsForSale').append(neededEmblems);
    }

    if (obj.Emblems.Needed.length > 0) {
        $('#emblemsMissing').empty();
        var factionDiv = "<div class=\"prose\"><h2><span>Still Missing</span></h2></div>";
        $.each(obj.Emblems.Needed, function(index, faction) {
            factionDiv += "<div class=\"items js-toggle\">";
            factionDiv += "<div class=\"prose\">";
            factionDiv += "<h3 class=\"items__control js-toggle-control is-active\">";
            factionDiv += faction.FactionName;
            factionDiv += "</h3>";
            factionDiv += "</div>";
            factionDiv += "<div class=\"grid items__content js-toggle-content\">";
            $.each(faction.Items, function(index, item) {
                factionDiv += "<div class=\"grid__item fablet-one-half tablet-one-third\">";
                factionDiv += "<div style=\"background-image: url(https://www.bungie.net" +
                    item.SecondaryIcon +
                    ")\" class=\"item\" data-id=\"";
                factionDiv += item.Hash;
                factionDiv += "\">";
                factionDiv += "<div class=\"grid grid--middled\">";
                factionDiv += "<div class=\"grid__item auto\">";
                factionDiv += "<div class=\"item__image\">";
                factionDiv += "<img src=\"https://www.bungie.net" +
                    item.Icon +
                    "\" alt=\"\" />";
                factionDiv += " </div>";
                factionDiv += " </div>";
                factionDiv += "<div class=\"grid__item one-half\">";
                factionDiv += "<div class=\"item__title\">";
                factionDiv += item.Name;
                factionDiv += " </div>";
                factionDiv += " </div>";
                factionDiv += " </div>";
                factionDiv += " </div>";
                factionDiv += " </div>";
            });

            factionDiv += "</div>";
            factionDiv += "</div>";
        });

        $('#emblemsMissing').append(factionDiv);
    }

    // Shaders
    var shaderExpiryDate = "<a id=\"shaders\" class=\"js-tab-heading\">Shaders</a><span data-date=\"2017-06-27T08:00:00Z\" role=\"timer\" class=\"countdown js-countdown\"><span class=\"countdown__title\">Next reset</span><span class=\"countdown__units js-countdown-units\">27 Jun 2017</span></span>";
    $('#shadersH1').html(shaderExpiryDate);
    processForSaleResults(obj.Shaders.ForSale, "shadersForSale");
    processNeededResults(obj.Shaders.Needed, "shadersMissing");

    // Sparrows
    var sparrowsExpiryDate = "<a id=\"sparrows\" class=\"js-tab-heading\">Sparrows</a><span data-date=\"2017-06-27T08:00:00Z\" role=\"timer\" class=\"countdown js-countdown\"><span class=\"countdown__title\">Next reset</span><span class=\"countdown__units js-countdown-units\">27 Jun 2017</span></span>";
    $('#sparrowsH1').html(sparrowsExpiryDate);
    processForSaleResults(obj.Sparrows.ForSale, "sparrowsForSale");
    processNeededResults(obj.Sparrows.Needed, "sparrowsMissing");

    // Ships
    var shipsExpiryDate = "<a id=\"ships\" class=\"js-tab-heading\">Ships</a><span data-date=\"2017-06-27T08:00:00Z\" role=\"timer\" class=\"countdown js-countdown\"><span class=\"countdown__title\">Next reset</span><span class=\"countdown__units js-countdown-units\">27 Jun 2017</span></span>";
    $('#shipsH1').html(shipsExpiryDate);
    processForSaleResults(obj.Ships.ForSale, "shipsForSale");
    processNeededResults(obj.Ships.Needed, "shipsMissing");

    $('.loading').hide();
}