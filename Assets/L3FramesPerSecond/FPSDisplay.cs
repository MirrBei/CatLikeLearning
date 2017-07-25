﻿using System;
using UnityEngine;
using UnityEngine.UI;

namespace L3FramesPerSecond
{
    [RequireComponent(typeof(FpsCounter))]
    public class FpsDisplay : MonoBehaviour
    {
        [Serializable]
        private struct FpsColor
        {
            public Color color;
            public int minimumFps;
        }

        [SerializeField] private Text highestFpsLabel;
        [SerializeField] private Text averageFpsLabel;
        [SerializeField] private Text lowestFpsLabel;
        [SerializeField] private FpsColor[] coloring;
        private FpsCounter fpsCounter;

        private static readonly string[] stringsFrom0To99 =
        {
            "00", "01", "02", "03", "04", "05", "06", "07", "08", "09",
            "10", "11", "02", "03", "04", "05", "06", "07", "08", "09",
            "20", "21", "02", "03", "04", "05", "06", "07", "08", "09",
            "30", "31", "32", "33", "34", "35", "36", "37", "38", "39",
            "40", "41", "42", "43", "44", "45", "46", "47", "48", "49",
            "50", "51", "52", "53", "54", "55", "56", "57", "58", "59",
            "60", "61", "62", "63", "64", "65", "66", "67", "68", "69",
            "70", "71", "72", "73", "74", "75", "76", "77", "78", "79",
            "80", "81", "82", "83", "84", "85", "86", "87", "88", "89",
            "90", "91", "92", "93", "94", "95", "96", "97", "98", "99"
        };

        private void Awake()
        {
            fpsCounter = GetComponent<FpsCounter>();
        }

        private void Update()
        {
            Display(highestFpsLabel, fpsCounter.HighestFps);
            Display(averageFpsLabel, fpsCounter.AverageFps);
            Display(lowestFpsLabel, fpsCounter.LowestFps);
        }

        private void Display(Text label, int fps)
        {
            label.text = stringsFrom0To99[Mathf.Clamp(fps, 0, 99)];
            for (var i = 0; i < coloring.Length; i++)
            {
                if (coloring[i].minimumFps > fps) continue;
                label.color = coloring[i].color;
                break;
            }
        }
    }
}