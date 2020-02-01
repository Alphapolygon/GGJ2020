using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

//convenient methods to validate string content
public static class StringExtension {

    public static bool AllWhiteSpace(this string s) {
        if (string.IsNullOrEmpty(s)) return true;
        for (int i = 0; i < s.Length; i++) {
            char c = s[i];
            if (!char.IsWhiteSpace(c)) return false;
        }
        return true;
    }

    public static bool IsNumber(this string s, bool allowDecimalPoint = true) {
        if (string.IsNullOrEmpty(s)) return false;
        for (int i = 0; i < s.Length; i++) {
            char c = s[i];
            if (char.IsWhiteSpace(c)) continue;
            if ((c == '.' || c == ',')) {
                if (!allowDecimalPoint) return false;
                continue;
            }
            if (!char.IsDigit(c)) return false;
        }
        return true;
    }

    public static bool IsUpperCase(this string s) {
        if (string.IsNullOrEmpty(s)) return false;
        for (int i = 0; i < s.Length; i++) {
            char c = s[i];
            if (char.IsWhiteSpace(c) || char.IsNumber(c) || c == '_') continue;
            if (!char.IsUpper(c)) return false;
        }
        return true;
    }

    public static bool IsLowerCase(this string s) {
        if (string.IsNullOrEmpty(s)) return false;
        for (int i = 0; i < s.Length; i++) {
            char c = s[i];
            if (char.IsWhiteSpace(c) || char.IsNumber(c) || c == '_') continue;
            if (!char.IsLower(c)) return false;
        }
        return true;
    }

    public static bool IsBooleanValue(this string s) {
        string _s = s.ToLower();
        return _s.StartsWith("true") || _s.StartsWith("false");
    }

    public static string StripWhiteSpace(this string s) {
        if (s.AllWhiteSpace()) return "";
        string result = "";
        int n = s.Length;
        for (int i = 0; i < n; i++) {
            char c = s[i];
            if (!char.IsWhiteSpace(c)) result += c;
        }
        return result;
    }

    public static bool ToBoolean(this string s) {
        string _s = s.ToLower();
        if (_s.StartsWith("true") || _s.StartsWith("false")) {
            return _s.Equals("true");
        } else {
#if UNITY_EDITOR
            Debug.Log("String value (" + s + ") is not boolean");
#endif
            return false;
        }
    }


    public static bool ContainsForbiddenNameCharacters(this string s) {
        int[] chars = StringInfo.ParseCombiningCharacters(s);
        int n = chars.Length;
        for (int i = 0; i < n; i++) {
            UnicodeCategory uc = CharUnicodeInfo.GetUnicodeCategory(s, chars[i]);
            if (   uc == UnicodeCategory.LowercaseLetter
                || uc == UnicodeCategory.UppercaseLetter
                || uc == UnicodeCategory.TitlecaseLetter
                || uc == UnicodeCategory.SpaceSeparator
                || uc == UnicodeCategory.DecimalDigitNumber
               ) {
                // character is ok, do nothing
            } else {
                // character is not ok
                return true;
            }
        }

        return false;
    }
}