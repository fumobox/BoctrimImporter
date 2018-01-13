using MPF.Domain;

namespace Boctrim.Domain
{
    /// <summary>
    /// An exctension of boct.
    /// </summary>
    /// <remarks>
    /// It isn't used in this version :)
    /// </remarks>
    public partial class BoctExtension: DomainModel
    {
        public const int EmptyId = -1;

        /// <summary>Get a local unique ID</summary>
        public int LUID { get; set; }

        public BoctExtension()
        {
        }

        public BoctExtension(BoctExtension data)
        {
            LUID = data.LUID;
        }

        public BoctExtension(ExtensionData data)
        {
            LUID = data.LUID;
        }

        public ExtensionData ToData()
        {
            var data = new ExtensionData();
            data.GUID = GUID;
            data.LUID = LUID;
            return data;
        }
        
    }
}
