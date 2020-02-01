#define USE_HASH_DEEP_SEARCH

using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

using Ideafixxxer.CsvParser;

namespace Kuuasema.Bike.Data {

public abstract class Database : ScriptableObject {

    private const string TAG = "[Database] ";

    protected const string DEFAULT_LINE_TAG = "DEFAULTS";

#region Deep Search

#if USE_HASH_DEEP_SEARCH
    private static Dictionary<int, DataEntry> deepSearchData = null;
#else
    private static Dictionary<string, DataEntry> deepSearchData = null;
#endif

    protected static void AddDeepSearchEntry(DataEntry entry) {
        if (entry == null) {
#if UNITY_EDITOR
            Debug.LogWarning(TAG + "Trying to add null entry to deep search data");
#endif
            return;
        }

#if LOG_INJECTIONS
        Debug.Log("Adding id (" + entry.Id + ")");
#endif

#if USE_HASH_DEEP_SEARCH
        if (deepSearchData == null) deepSearchData = new Dictionary<int, DataEntry>();
        int hash = IdToHash(entry.Id);
        if (deepSearchData.Keys.Count > 0) {
            if (deepSearchData.ContainsKey(hash)) {
#if UNITY_EDITOR
                var conflictingItem = deepSearchData[hash];
                Debug.LogWarning(TAG + "Trying to add multiple instances of key (" + entry.Id + ") to deep-search data.\nConflicting item is (" + conflictingItem + ")");
#endif
                return;
            }
        }
        deepSearchData.Add(hash, entry);
#else
        if (deepSearchData == null) deepSearchData = new Dictionary<string, DataEntry>();
        if (deepSearchData.Keys.Count > 0) {
            if (deepSearchData.ContainsKey(entry.Id)) {
#if UNITY_EDITOR
                var conflictingItem = deepSearchData[entry.Id];
                Debug.LogWarning(TAG + "Trying to add multiple instances of key (" + entry.Id + ") to deep-search data.\nConflicting item is (" + conflictingItem + ")");
#endif
                return;
            }
        }
        deepSearchData.Add(entry.Id, entry);
#endif

    }

    public static void ClearDeepSearchData() {
        if (deepSearchData != null)
            deepSearchData.Clear();
    }

    public static DataEntry DeepSearch(string id) {
        if (string.IsNullOrEmpty(id)) {
#if UNITY_EDITOR
            Debug.LogWarning(TAG + "Empty id passed to DeepSearch");
#endif
            return null;
        }
        if (deepSearchData == null) return null;

#if USE_HASH_DEEP_SEARCH
        int hash = IdToHash(id);        
        if (deepSearchData.ContainsKey(hash)) return deepSearchData[hash];
#else
        if (deepSearchData.ContainsKey(id)) return deepSearchData[id];
#endif

        return null;
    }

    private static int IdToHash(string id) {
        return Animator.StringToHash(id);
    }

#endregion

    [SerializeField]
    protected TextAsset csvFile;
#if UNITY_EDITOR
    [SerializeField]
    private bool forceLocalCsv = false;
#endif
    public bool ForceLocalCsv { 
        get {  
#if UNITY_EDITOR
            return forceLocalCsv;
#else
            return false;
#endif
        } 
#if UNITY_EDITOR
        set {
            forceLocalCsv = value;
        }
#endif
    }

#if UNITY_EDITOR
    //for inspectors
    public TextAsset CsvFile { get { return csvFile; } set { csvFile = value; } }
#endif

    internal string Filename { 
        get { 
            if (csvFile == null) return null;
            return csvFile.name;
        }
    }

    protected bool loaded = false;

    protected List<DataEntry> cachedDataRef = null;

    protected bool csvReadOk = false;
    protected string[] defaultLine;
    protected string[][] csvData;

    #region Tasks

    private List<List<UnityAction>> deferredTasks = null;

    public bool TasksInQueue { get { return deferredTasks != null && deferredTasks.Count > 0; } }

    //schedule task to be updated in next frame
    public void ScheduleTask(UnityAction task, int priority = 0) {
        if (deferredTasks == null) deferredTasks = new List<List<UnityAction>>();
        while (deferredTasks.Count <= priority) deferredTasks.Add(new List<UnityAction>());
        deferredTasks[priority].Add(task);
    }

    public void ExecuteTasks() {
        if (deferredTasks != null && deferredTasks.Count > 0) {
            for (int i = 0; i < deferredTasks.Count; i++) {
                foreach (var t in deferredTasks[i]) {
                    var task = t;
                    task();
                }
                deferredTasks[i].Clear();
            }
            deferredTasks.Clear();
        }
    }

    #endregion

    #region Abstracts

    internal abstract void Load(string csvContent = null);
    protected abstract List<DataEntry> Data { get; }

    #endregion
    
    protected void ReadCsv(TextAsset file) {
        if (file == null) {
#if UNITY_EDITOR
            Debug.LogWarning(TAG + "No csv file defined in database (" + name + ")");
#endif
            return;
        }
        ReadCsv(file.text);
    }

    protected void ReadCsv(string data) {
        csvReadOk = false;
        csvData = null;
        defaultLine = null;

        string[] lines = data.Split(new char[] { '\n' });
        if (lines != null && lines.Length > 0) {
            //parse csv
            csvData = new CsvParser().Parse(lines);

            //find default data
            foreach (var line in csvData) {
                if (IsEmptyLine(line)) continue;
                if (line[0].Equals(DEFAULT_LINE_TAG)) {
                    defaultLine = line;
                    break;
                }
            }
            if (defaultLine == null) {
#if UNITY_EDITOR
                Debug.LogError(TAG + "Couldn't find default values line from csv " + csvFile.name);
#endif
                return;
            }
            csvReadOk = true;
        }
    }

    protected static bool IsEmptyLine(string[] line) {
        if (line == null || line.Length < 1) return true;
        int c = 0;
        foreach (var l in line) {
            if (l.AllWhiteSpace()) ++c;
        }
        return c == line.Length;
    }

    public virtual DataEntry Find(string id) {
        if (string.IsNullOrEmpty(id)) {
#if UNITY_EDITOR
            Debug.LogWarning(TAG + "id is empty when doing find in database");
#endif
            return null;
        }

#if UNITY_EDITOR
        if (!id.IsUpperCase()) Debug.LogWarning(TAG + "Item id (" + id + ") should be uppercase");
#endif
        var data = Data;
        if (data == null || data.Count < 1) {
#if UNITY_EDITOR
            Debug.LogWarning(TAG + "Data in database (" + name + ") is empty");
#endif
            return null;
        }
        int n = data.Count;
        for (int i = 0; i < n; i++) {
            var a = data[i];
            if (a.Id.Equals(id)) return a;
        }
        return null;
    }

}

}
