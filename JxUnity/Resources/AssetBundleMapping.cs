﻿using System;
using System.Collections;
using System.Collections.Generic;

namespace JxUnity.Resources
{
    internal class AssetBundleMapping : IEnumerable<KeyValuePair<string, AssetBundleMapping.MappingItem>>
    {
        public class MappingItem
        {
            public string assetPath { get; private set; }
            public string assetName { get; private set; }
            public string assetPackageName { get; private set; }

            public MappingItem(string assetPath, string assetName, string assetBundleName)
            {
                this.assetPath = assetPath;
                this.assetName = assetName;
                this.assetPackageName = assetBundleName;
            }
        }

        private Dictionary<string, MappingItem> mappingTable;

        public AssetBundleMapping(string content)
        {
            this.mappingTable = new Dictionary<string, MappingItem>();

            string[] mappingLines = content.Split('\n');
            foreach (string item in mappingLines)
            {
                if (string.IsNullOrEmpty(item))
                {
                    continue;
                }
                string[] relation = item.Trim().Split(':');
                var assetPath = relation[0].ToLower();
                var assetName = relation[1].ToLower();
                var abName = relation[2];
                if (this.mappingTable.ContainsKey(assetPath))
                {
                    throw new ArgumentException("AssetMapping: asset path exist: " + assetPath);
                }
                this.mappingTable.Add(assetPath, new MappingItem(assetPath, assetName, abName));
            }
        }

        public MappingItem Mapping(string assetPath)
        {
            MappingItem item = null;
            if (this.mappingTable.TryGetValue(assetPath, out item))
            {
                return item;
            }
            return null;
        }

        public IEnumerator<KeyValuePair<string, MappingItem>> GetEnumerator()
        {
            return this.mappingTable.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.mappingTable.GetEnumerator();
        }
    }
}