using System;
using System.Drawing;
using System.IO;
using System.Text;

namespace MUGENCharsSet
{
    #region Sprite file class

    /// <summary>
    /// Sprite file class
    /// </summary>
    public class SpriteFile
    {
        /// <summary>SFF file version enumeration</summary>
        public enum SffVerion { Unknown, V1_01, V1_02, V2_00 };

        /// <summary>SFF V1.01 head byte size</summary>
        public const int HeaderSizeV1_01 = 33;

        /// <summary>SFF V2.00 head byte size</summary>
        public const int HeaderSizeV2_00 = 53;

        private string _sffPath;
        private string _signature;
        private SffVerion _version;
        private int _numberOfGroups;
        private int _numberOfImages;
        private int _firstSubheaderOffset;
        private int _subheaderSize;
        private bool _sharedPalette;
        private SpriteFileSubNode _firstNode;

        /// <summary>
        /// Get an absolute path of SFF files
        /// </summary>
        public string SffPath
        {
            get { return _sffPath; }
        }

        /// <summary>
        /// Get Signature
        /// </summary>
        public string Signature
        {
            get { return _signature; }
        }

        /// <summary>
        /// Get SFF file versions
        /// </summary>
        public SffVerion Version
        {
            get { return _version; }
        }

        /// <summary>
        /// Get Number of groups
        /// </summary>
        public int NumberOfGroups
        {
            get { return _numberOfGroups; }
        }

        /// <summary>
        /// Get Number of images
        /// </summary>
        public int NumberOfImages
        {
            get { return _numberOfImages; }
        }

        /// <summary>
        /// Get the offset of the first child node
        /// </summary>
        public int FirstSubheaderOffset
        {
            get { return _firstSubheaderOffset; }
        }

        /// <summary>
        /// Gets size of subheader in bytes
        /// </summary>
        public int SubheaderSize
        {
            get { return _subheaderSize; }
        }

        /// <summary>
        /// Get a Palette type
        /// </summary>
        public bool SharedPalette
        {
            get { return _sharedPalette; }
        }

        /// <summary>
        /// Get the first child node
        /// </summary>
        public SpriteFileSubNode FirstSubNode
        {
            get
            {
                if (_firstNode == null)
                {
                    try
                    {
                        _firstNode = new SpriteFileSubNode(SffPath, FirstSubheaderOffset, Version);
                    }
                    catch (ApplicationException)
                    {
                        return null;
                    }
                }
                return _firstNode;
            }
        }

        /// <summary>
        /// Create according to the specified SFF file path<see cref="SpriteFile"/>New instance
        /// </summary>
        /// <param name="path">SFF file absolute path</param>
        /// <exception cref="System.ApplicationException"></exception>
        public SpriteFile(string path)
        {
            _sffPath = path;
            FileStream fs = null;
            BinaryReader br = null;
            try
            {
                fs = new FileStream(SffPath, FileMode.Open);
                fs.Position = 12;
                br = new BinaryReader(fs);
                byte[] data = br.ReadBytes(4);
                fs.Position = 0;
                string version = String.Format("{0}{1}{2}{3}", data[0], data[1], data[2], data[3]);
                switch (version)
                {
                    case "0101":
                        _version = SffVerion.V1_01;
                        break;

                    case "0102":
                        _version = SffVerion.V1_02;
                        break;

                    case "0002":
                        _version = SffVerion.V2_00;
                        break;

                    default:
                        _version = SffVerion.Unknown;
                        break;
                }

                if (Version == SffVerion.V1_01) ReadHeaderV1_01(br);
                else if (Version == SffVerion.V1_02 || Version == SffVerion.V2_00) ReadHeaderV2_00(br);
            }
            catch (Exception)
            {
                throw new ApplicationException("Read the SFF file failed！");
            }
            finally
            {
                if (br != null) br.Close();
                if (fs != null) fs.Close();
            }
        }

        /// <summary>
        /// Read SFF V1.01 version of the header information
        /// </summary>
        /// <param name="br">Binary data stream of the current SFF file</param>
        /// <exception cref="System.ApplicationException"></exception>
        private void ReadHeaderV1_01(BinaryReader br)
        {
            byte[] data = br.ReadBytes(HeaderSizeV1_01);
            if (data.Length != HeaderSizeV1_01) throw new ApplicationException("Data size error！");
            _signature = Encoding.Default.GetString(data, 0, 11);
            _numberOfGroups = BitConverter.ToInt32(data, 16);
            _numberOfImages = BitConverter.ToInt32(data, 20);
            _firstSubheaderOffset = BitConverter.ToInt32(data, 24);
            _subheaderSize = BitConverter.ToInt32(data, 28);
            _sharedPalette = data[32] > 0;
        }

        /// <summary>
        /// Read the SFF V2.00 version of the header information
        /// </summary>
        /// <param name="br">Binary data stream of the current SFF file</param>
        /// <exception cref="System.ApplicationException"></exception>
        private void ReadHeaderV2_00(BinaryReader br)
        {
            byte[] data = br.ReadBytes(HeaderSizeV2_00);
            if (data.Length != HeaderSizeV2_00) throw new ApplicationException("Data size error！");
            _firstSubheaderOffset = BitConverter.ToInt32(data, 36);
            _numberOfImages = BitConverter.ToInt32(data, 40);
        }

        /// <summary>
        /// Get formatted version information
        /// </summary>
        /// <param name="version">SFF version</param>
        /// <returns>Formatted version information</returns>
        public static string GetFormatVersion(SffVerion version)
        {
            switch (version)
            {
                case SffVerion.V1_01: return "1.01";
                case SffVerion.V1_02: return "1.02";
                case SffVerion.V2_00: return "2.00";
                default: return "unknown";
            }
        }
    }

    #endregion Sprite file class

    #region Sprite file sub-node class

    /// <summary>
    /// Sprite file sub-node class
    /// </summary>
    public class SpriteFileSubNode
    {
        /// <summary>SFF V1.01 child node head byte size</summary>
        public const int HeaderSizeV1_01 = 32;

        /// <summary>SFF V2.00 child node head byte size</summary>
        public const int HeaderSizeV2_00 = 28;

        private int _nextOffset;
        private int _imageSize;
        private Point _axis;
        private int _groupNumber;
        private int _imageNumber;
        private int _sharedIndex;
        private bool _copyLastPalette;
        private byte[] _imageData;

        /// <summary>
        /// Get the offset of the next child node
        /// </summary>
        public int NextOffset
        {
            get { return _nextOffset; }
        }

        /// <summary>
        /// Get image data byte size
        /// </summary>
        public int ImageSize
        {
            get { return _imageSize; }
        }

        /// <summary>
        /// Get image AXIS coordinates
        /// </summary>
        public Point Axis
        {
            get { return _axis; }
        }

        /// <summary>
        /// Get groups number
        /// </summary>
        public int GroupNumber
        {
            get { return _groupNumber; }
        }

        /// <summary>
        /// Get image number (in the group)
        /// </summary>
        public int ImageNumber
        {
            get { return _imageNumber; }
        }

        /// <summary>
        /// Get Index of previous copy of sprite (linked sprites only)
        /// </summary>
        public int SharedIndex
        {
            get { return _sharedIndex; }
        }

        /// <summary>
        /// Get True if palette is same as previous (or first) image
        /// </summary>
        public bool CopyLastPalette
        {
            get { return _copyLastPalette; }
        }

        /// <summary>
        /// Get image data
        /// </summary>
        public byte[] ImageData
        {
            get { return _imageData; }
        }

        /// <summary>
        /// Path according to the specified SFF file、File flow offset and SFF file version creation<see cref="SpriteFileSubNode"/>New instance
        /// </summary>
        /// <param name="path">SFF file absolute path</param>
        /// <param name="offset">Child node offset</param>
        /// <param name="version">SFF file version</param>
        /// <exception cref="System.ApplicationException"></exception>
        public SpriteFileSubNode(string path, int offset, SpriteFile.SffVerion version)
        {
            FileStream fs = null;
            BinaryReader br = null;
            try
            {
                fs = new FileStream(path, FileMode.Open);
                fs.Position = offset;
                br = new BinaryReader(fs);
                if (version == SpriteFile.SffVerion.V1_01) ReadV1_01(br);
                else if (version == SpriteFile.SffVerion.V1_02 || version == SpriteFile.SffVerion.V2_00) ReadV2_00(br);
                else throw new Exception();
            }
            catch (Exception)
            {
                throw new ApplicationException("Read the SFF child node failed！");
            }
            finally
            {
                if (br != null) br.Close();
                if (fs != null) fs.Close();
            }
        }

        /// <summary>
        /// Read SFF V1.01 version node information
        /// </summary>
        /// <param name="br">Binary data stream of the current SFF file</param>
        /// <exception cref="System.ApplicationException"></exception>
        private void ReadV1_01(BinaryReader br)
        {
            byte[] data = br.ReadBytes(HeaderSizeV1_01);
            if (data.Length != HeaderSizeV1_01) throw new ApplicationException("Data size error！");
            _nextOffset = BitConverter.ToInt32(data, 0);
            _imageSize = BitConverter.ToInt32(data, 4);
            if (ImageSize == 0) throw new Exception();
            _axis = new Point(BitConverter.ToInt16(data, 8), BitConverter.ToInt16(data, 10));
            _groupNumber = BitConverter.ToInt16(data, 12);
            _imageNumber = BitConverter.ToInt16(data, 14);
            _sharedIndex = BitConverter.ToInt16(data, 16);
            _copyLastPalette = data[18] > 0;
            _imageData = br.ReadBytes(ImageSize);
        }

        /// <summary>
        /// Read the SFF V2.00 version node information
        /// </summary>
        /// <param name="br">Binary data stream of the current SFF file</param>
        /// <exception cref="System.ApplicationException"></exception>
        private void ReadV2_00(BinaryReader br)
        {
            byte[] data = br.ReadBytes(HeaderSizeV2_00);
            if (data.Length != HeaderSizeV2_00) throw new ApplicationException("Data size error！");
            _groupNumber = BitConverter.ToInt16(data, 0);
            _imageNumber = BitConverter.ToInt16(data, 2);
            _axis = new Point(BitConverter.ToInt16(data, 8), BitConverter.ToInt16(data, 10));
            _imageSize = BitConverter.ToInt32(data, 20);
        }
    }

    #endregion Sprite file sub-node class
}