using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.ApplicationModel.Resources;

namespace Fotografix.Editor.UI
{
    /// <summary>
    /// Represents the items displayed in the blend mode drop-down list, including
    /// separators between groups of blend modes.
    /// </summary>
    public sealed class BlendModeList : ReadOnlyCollection<BlendModeListItem>
    {
        /// <summary>
        /// Each of these items marks the start of a new group in the blend mode list.
        /// </summary>
        public static readonly ISet<BlendMode> GroupMarkers = new HashSet<BlendMode> {
            BlendMode.Darken,
            BlendMode.Lighten,
            BlendMode.Overlay,
            BlendMode.Difference,
            BlendMode.Hue
        };

        private BlendModeList(IList<BlendModeListItem> list) : base(list)
        {
        }

        /// <summary>
        /// Gets the <see cref="BlendModeListItem"/> corresponding to the specified <see cref="BlendMode"/>.
        /// </summary>
        public BlendModeListItem this[BlendMode blendMode]
        {
            get
            {
                return this.Where(item => item.BlendMode == blendMode).First();
            }
        }

        /// <summary>
        /// Returns the index of the specified <see cref="BlendMode"/> within the list.
        /// </summary>
        public int IndexOf(BlendMode blendMode)
        {
            for (int i = 0; i < Count; i++)
            {
                if (this[i].BlendMode == blendMode)
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Creates a <see cref="BlendModeList"/> that contains an element for each <see cref="BlendMode"/>.
        /// </summary>
        public static BlendModeList Create()
        {
            var resourceLoader = ResourceLoader.GetForViewIndependentUse("Fotografix.Editor/Resources/BlendMode");
            var items = new List<BlendModeListItem>();

            foreach (BlendMode blendMode in Enum.GetValues(typeof(BlendMode)))
            {
                if (GroupMarkers.Contains(blendMode))
                {
                    items.Add(BlendModeListItem.Separator);
                }

                items.Add(new BlendModeListItem(blendMode, resourceLoader.GetString(blendMode.ToString())));
            }

            return new BlendModeList(items);
        }
    }
}
