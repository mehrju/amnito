var transformNumbers = (function () {
    var numerals = {
        persian: ["۰", "۱", "۲", "۳", "۴", "۵", "۶", "۷", "۸", "۹"],
        arabic: ["٠", "١", "٢", "٣", "٤", "٥", "٦", "٧", "٨", "٩"]
    };
    function fromEnglish(str, lang) {
        var i, len = str.length, result = "";

        for (i = 0; i < len; i++)
            result += numerals[lang][str[i]];
        return result;
    }
    return {
        toNormal: function (str) {
            var num, i, len = str.length, result = "";

            for (i = 0; i < len; i++) {
                num = numerals["persian"].indexOf(str[i]);
                num = num != -1 ? num : numerals["arabic"].indexOf(str[i]);
                if (num == -1) num = str[i];
                result += num;
            }
            return result;
        },
        toPersian: function (str, lang) {
            return fromEnglish(str, "persian");
        },
        toArabic: function (str) {
            return fromEnglish(str, "arabic");
        }
    }
})();
function ToJsutNumber(value) {

    var hasZero = false;
    var zeroCount = 0;
    if (value.startsWith("0") && value.length > 1) {
        hasZero = true;
        for (var i = 0; i < value.length; i++) {
            if (value[i] == '0')
                zeroCount++;
            else
                break;
        }
        if (zeroCount && zeroCount > 0)
            value = value.slice(zeroCount, value.length);
    }
    var value = transformNumbers.toNormal(value);
    var result = (value.replace(/[^\d]+/gi, ''));
    if (hasZero) {
        var p = '0';
        result = p.repeat(zeroCount) + (result.toString());
    }
    return result;
}
function ToLocalInt(value) {

    var hasZero = false;

    var value = transformNumbers.toNormal(value);
    var result = (parseInt(value.replace(/[^\d]+/gi, '')) || 0);
    if (hasZero) {
        result = '0' + (result.toString());
    }
    return result.toLocaleString('en-US');
}