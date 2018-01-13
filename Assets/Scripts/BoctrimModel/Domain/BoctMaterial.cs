using UnityEngine;
using System.Collections;
using MPF.Domain;

namespace Boctrim.Domain
{

    public partial class BoctMaterial: DomainModel
    {
        int _id = EmptyId;

        public const int EmptyId = -1;

        public const int DefaultId = 0;

        /// <summary>
        /// Local Unique ID
        /// </summary>
        public int LUID
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
                MoveId = value;
            }
        }

        public Color Color { get; set;}

        public int SortOrder { get; set; }

        public int BoctCount { get; set;}

        public int MoveId { get; set;}

        public bool Dirty
        {
            get
            {
                return (LUID != MoveId) || ColorChanged;
            }
        }

        public bool Disposed
        {
            get
            {
                return LUID != MoveId;
            }
        }

        public bool ColorChanged { get; set;}

        #region Static Property

        public static Color DefaultColor
        {
            get
            {
                return new Color(0.5f, 0.5f, 0.5f, 1);
            }
        }

        #endregion

        public BoctMaterial()
        {
            MoveId = EmptyId;
        }

        /// <summary>
        /// Clone an instance.
        /// </summary>
        public BoctMaterial(BoctMaterial mat)
        {
            GUID = mat.GUID;
            LUID = mat.LUID;
            Color = mat.Color;
            SortOrder = mat.SortOrder;
            BoctCount = mat.BoctCount;
        }

        public BoctMaterial(MaterialData data)
        {
            GUID = data.GUID;
            LUID = data.LUID;
            Color = data.Color.ToColor();
            SortOrder = data.SortOrder;
        }

        public override string ToString()
        {
            return GUID + " " + LUID + " " + MoveId + " " + Color.ToString();
        }

        public MaterialData ToData()
        {
            var data = new MaterialData();
            data.GUID = GUID;
            data.LUID = LUID;
            data.Color = Color.ToData();
            return data;
        }
        
    }

}