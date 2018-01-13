using System.Text;
using UniRx;
using System.Collections.Generic;
using MPF.Domain;

namespace Boctrim.Domain
{

    /// <summary>
    /// The blob of blocks.
    /// </summary>
    public partial class BoctRegion: DomainModel
    {
        /// <summary>Region state</summary>
        public enum State
        {
            /// <summary>The region is ready.</summary>
            Ready,
            /// <summary>The region needs update.</summary>
            Dirty,
            /// <summary>The region is disposed.</summary>
            Disposed
        }

        /// <summary>Get a local unique ID</summary>
        public int LUID { get; set;}

        /// <summary>Get a head boct.</summary>
        public Boct Head { get; set;}

        /// <summary>Get a current region state.</summary>
        public ReactiveProperty<State> CurrentState { get; set;}

        public const int EmptyId = -10000;

        public bool Disposed { get; private set;}

        public Dictionary<int, int> MaterialCounts { get; set;}

        public BoctRegion()
        {
            CurrentState = new ReactiveProperty<State>();
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("[Region]\n");
            sb.Append("GUID: " + GUID + "\n");
            sb.Append("LUID: " + LUID + "\n");
            return sb.ToString();
        }

        /// <summary>
        /// Get a boct count.
        /// </summary>
        public int Count
        {
            get
            {
                return Head.SolidCount;
            }
        }

    }
}