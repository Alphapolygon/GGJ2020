using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kuuasema.Bike.Data { 

public class DataEntry {
    
    protected const int COLUMN_ID = 0;    

    public static readonly char[] TRIMMED_CHARS = { '\n', '\r', ' ' };
    protected const char MULTI_ENTRY_SEPARATOR = ';';
    protected const char KEYVALUE_SEPARATOR = ':';

    protected string id;
    public string Id { get { return id; } }

    private static bool HasKey(string[] csvLine, int index) {
        if (csvLine == null || csvLine.Length < (index + 1)) return false;
        return !string.IsNullOrEmpty(csvLine[index]) && !csvLine[index].AllWhiteSpace();
    }

    protected static string ReadKey(int keyIndex, string[] csvLine, string[] defaults) {
        return HasKey(csvLine, keyIndex) ? csvLine[keyIndex].Trim(TRIMMED_CHARS) : defaults[keyIndex].Trim(TRIMMED_CHARS);
    }

}

}