﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct LocalisedString {
    public string key;

    public LocalisedString(string key) {
        this.key = key;
    }

    public string value {
        get { return LocalisationSystem.GetLocalisedValue(key); }
        set { key = value; }
    }

    public static implicit operator LocalisedString(string key) {
        return new LocalisedString(key);
    }
}
