using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using blz;
using CTR;

namespace Fire_Emblem_If_Patcher
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            TmpDir = Path.GetDirectoryName(Application.ExecutablePath) + Path.DirectorySeparatorChar + "tmp" + Path.DirectorySeparatorChar;
            string cd = Path.GetDirectoryName(Application.ExecutablePath) + Path.DirectorySeparatorChar;
            if (!File.Exists(cd + "rom.rsf"))
            {
                File.WriteAllBytes(cd + "rom.rsf", Properties.Resources.rom);
            }
            if (!File.Exists(cd + "makerom.exe"))
            {
                File.WriteAllBytes(cd + "makerom.exe", Properties.Resources.makerom);
            }
        }

        public volatile int threads = 0;
        internal static bool Card2;
        private string Exe_NormPad;
        private string Exe_AltPad;
        private string Rom_Pad;
        private string Ex_Pad;
        private string Serial;

        private string TmpDir;

        private byte[] Logo = new byte[0x2000];

        internal static DialogResult Alert(params string[] lines)
        {
            SystemSounds.Asterisk.Play();
            string msg = String.Join(Environment.NewLine + Environment.NewLine, lines);
            return MessageBox.Show(msg, "Alert", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        internal static DialogResult Prompt(MessageBoxButtons btn, params string[] lines)
        {
            SystemSounds.Question.Play();
            string msg = String.Join(Environment.NewLine + Environment.NewLine, lines);
            return MessageBox.Show(msg, "Prompt", btn, MessageBoxIcon.Asterisk);
        }

        private void B_ROM_Click(object sender, EventArgs e)
        {
            if (threads > 0) { Alert("Please wait for all operations to finish first."); return; }
            TB_ROM.Text = string.Empty;
            DisablePads();
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() != DialogResult.OK) return;
            FileInfo fi = new FileInfo(ofd.FileName);
            if (fi.Length < 0x4200)
            {
                MessageBox.Show("ROM is invalid.");
                return;
            }
            string magic;
            try
            {
                using (BinaryReader br = new BinaryReader(File.OpenRead(ofd.FileName)))
                {
                    br.BaseStream.Seek(0x100, SeekOrigin.Begin);
                    magic = new string(br.ReadBytes(4).Select(c => (char)c).ToArray());
                    if (magic != "NCSD")
                    {
                        MessageBox.Show("Provided file is not a valid .3DS ROM.");
                        return;
                    }
                    br.BaseStream.Seek(0x4100, SeekOrigin.Begin);
                    magic = new string(br.ReadBytes(4).Select(c => (char)c).ToArray());
                    if (magic != "NCCH")
                    {
                        MessageBox.Show("Provided file is not a valid .3DS ROM.");
                        return;
                    }
                }
            }
            catch
            {
                MessageBox.Show("Failed to read the provided file. Try again?");
                return;
            }
            TB_ROM.Text = ofd.FileName;
            B_Validate.Enabled = (TB_ROM.Text != string.Empty && TB_XorpadDir.Text != string.Empty && TB_PatchDir.Text != string.Empty);
        }

        private void B_Xorpads_Click(object sender, EventArgs e)
        {
            if (threads > 0) { Alert("Please wait for all operations to finish first."); return; }
            DisablePads();
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() != DialogResult.OK) return;

            DirectoryInfo di = new DirectoryInfo(fbd.SelectedPath);
            if (!di.Exists)
            {
                MessageBox.Show("Selected Directory does not exist.");
                return;
            }
            var files = di.GetFiles();
            string[] pads = new string[] { "Main.exheader", "Main.exefs_7x", "Main.exefs_norm", "Main.romfs" };
            bool HasAllPads = true;
            foreach (string padname in pads)
            {
                HasAllPads &= files.Where(fi => fi.Name.Contains(padname)).Count() == 1;
                if (!HasAllPads)
                {
                    MessageBox.Show("You are missing a xorpad: " + padname);
                    return;
                }
            }
            Ex_Pad = files.Where(fi => fi.Name.Contains(pads[0])).Select(fi => fi.FullName).First();
            Exe_AltPad = files.Where(fi => fi.Name.Contains(pads[1])).Select(fi => fi.FullName).First();
            Exe_NormPad = files.Where(fi => fi.Name.Contains(pads[2])).Select(fi => fi.FullName).First();
            Rom_Pad = files.Where(fi => fi.Name.Contains(pads[3])).Select(fi => fi.FullName).First();
            TB_XorpadDir.Text = fbd.SelectedPath + Path.DirectorySeparatorChar;
            B_Validate.Enabled = (TB_ROM.Text != string.Empty && TB_XorpadDir.Text != string.Empty && TB_PatchDir.Text != string.Empty);
        }

        private void B_Patch_Click(object sender, EventArgs e)
        {
            if (threads > 0) { Alert("Please wait for all operations to finish first."); return; }
            DisablePads();
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() != DialogResult.OK) return;

            DirectoryInfo di = new DirectoryInfo(fbd.SelectedPath);
            if (!di.Exists)
            {
                MessageBox.Show("Selected Directory does not exist.");
                return;
            }

            var directories = di.GetDirectories();
            if (directories.Length != 2)
            {
                MessageBox.Show("Selected Patch Directory has too many or too few subfolders.");
                return;
            }
            if (directories[0].Name != "exe")
            {
                MessageBox.Show("Selected Patch Directory has no 'exe' folder.");
                return;
            }
            if (directories[1].Name != "rom")
            {
                MessageBox.Show("Selected Patch Directory has no 'rom' folder.");
                return;
            }

            TB_PatchDir.Text = fbd.SelectedPath + Path.DirectorySeparatorChar;
            B_Validate.Enabled = (TB_ROM.Text != string.Empty && TB_XorpadDir.Text != string.Empty && TB_PatchDir.Text != string.Empty);
        }

        private void CHK_Card2_CheckedChanged(object sender, EventArgs e)
        {
            Card2 = CHK_Card2.Checked;
            if (Card2 == true)
            {
                MessageBox.Show("Note: Fire Emblem: If is normally a CARD1 game. CARD2 works, but is not recommended.");
            }
        }

        private void DisablePads()
        {
            B_Validate.Enabled =
            B_3DS.Enabled = 
            B_CIA.Enabled = 
            B_3DS.Visible = 
            B_CIA.Visible = false;
        }

        private void B_Validate_Click(object sender, EventArgs e)
        {
            if (threads > 0) { Alert("Please wait for all operations to finish first."); return; }
            string PAD_DIR = TB_XorpadDir.Text;
            string ROM_PATH = TB_ROM.Text;

            new Thread(() =>
                {
                    threads++;
                    SetEnabled(false, B_Validate);
                    if (ValidatePads(PAD_DIR, ROM_PATH))
                    {
                        SetEnabled(true, B_3DS, B_CIA);
                        SetVisible(true, B_3DS, B_CIA);
                    }
                    else
                    {
                        SetEnabled(false, B_3DS, B_CIA);
                        SetVisible(false, B_3DS, B_CIA);
                    }
                    SetEnabled(true, B_Validate);
                    threads--;
                }).Start();
        }

        private bool ValidatePads(string PAD_DIR, string ROM_PATH)
        {
            CTR_ROM.updateTB(RTB_Progress, "Validating ROM and Xorpads...");
            if (Directory.Exists(TmpDir))
                DeleteDirectory(TmpDir);
            Directory.CreateDirectory(TmpDir);
            byte[] NCCH_Header = new byte[0x200];
            using (var fs = File.OpenRead(ROM_PATH))
            {
                fs.Seek(0x4000, SeekOrigin.Begin);
                fs.Read(NCCH_Header, 0, NCCH_Header.Length);
            }
            Serial = Encoding.ASCII.GetString(NCCH_Header, 0x150, 10);
            if (!Serial.StartsWith("CTR-P-BF") || !Serial.EndsWith("J"))
            {
                CTR_ROM.updateTB(RTB_Progress, "Provided ROM is not a copy of Fire Emblem: If.");
                return false;
            }

            byte[] ExheaderHash = new byte[0x20];
            byte[] ExeSuperHash = new byte[0x20];
            byte[] RomSuperHash = new byte[0x20];
            Array.Copy(NCCH_Header, 0x160, ExheaderHash, 0x0, 0x20);
            Array.Copy(NCCH_Header, 0x1C0, ExeSuperHash, 0x0, 0x20);
            Array.Copy(NCCH_Header, 0x1E0, RomSuperHash, 0x0, 0x20);
            uint RomHashSize = 0x200 * BitConverter.ToUInt32(NCCH_Header, 0x1B8);
            uint ExeHashSize = 0x200 * BitConverter.ToUInt32(NCCH_Header, 0x1A8);
            uint ExhHashSize = 0x400;
            uint RomOffset = 0x4000 + 0x200 * BitConverter.ToUInt32(NCCH_Header, 0x1B0);
            uint ExeOffset = 0x4000 + 0x200 * BitConverter.ToUInt32(NCCH_Header, 0x1A0);

            uint ExeLen = 0x200 * BitConverter.ToUInt32(NCCH_Header, 0x1A4);
            uint RomLen = BitConverter.ToUInt32(NCCH_Header, 0x1B4);
            uint ExhOffset = 0x4000 + 0x200;
            uint LogoOffset = 0x4000 + 0x200 * BitConverter.ToUInt32(NCCH_Header, 0x198);
            using (var ROM = File.OpenRead(ROM_PATH))
            {
                using (var ROMPAD = File.OpenRead(Rom_Pad))
                {
                    byte[] EncSuper = new byte[RomHashSize];
                    byte[] SuperPad = new byte[RomHashSize];
                    byte[] DecSuper = new byte[RomHashSize];
                    ROM.Seek(RomOffset, SeekOrigin.Begin);
                    ROM.Read(EncSuper, 0, EncSuper.Length);
                    ROMPAD.Seek(0, SeekOrigin.Begin);
                    ROMPAD.Read(SuperPad, 0, SuperPad.Length);
                    for (int i = 0; i < EncSuper.Length; i++)
                        DecSuper[i] = (byte)(EncSuper[i] ^ SuperPad[i]);
                    byte[] CalcHash;
                    using (SHA256Managed sha = new SHA256Managed())
                        CalcHash = sha.ComputeHash(DecSuper);
                    if (!CalcHash.SequenceEqual(RomSuperHash) || Encoding.ASCII.GetString(DecSuper, 0, 4) != "IVFC")
                    {
                        CTR_ROM.updateTB(RTB_Progress, "Provided ROMFS or ROMFS Xorpad is invalid.");
                        return false;
                    }

                }
                byte[] Exe_Normal = File.ReadAllBytes(Exe_NormPad);
                byte[] Exe_Alt = File.ReadAllBytes(Exe_AltPad);
                byte[] EncExe = new byte[ExeLen];
                ROM.Seek(ExeOffset, SeekOrigin.Begin);
                ROM.Read(EncExe, 0, EncExe.Length);
                byte[] DecExe = new byte[ExeLen];
                for (int i = 0; i < EncExe.Length; i++)
                {
                    DecExe[i] = (byte)(EncExe[i] ^ Exe_Normal[i]);
                }
                for (int i = 0; i < 10; i++)
                {
                    string fileName = Encoding.ASCII.GetString(DecExe, 0x10 * i, 8).TrimEnd((char)0);
                    if (fileName == ".code")
                    {
                        uint code_ofs = BitConverter.ToUInt32(DecExe, 0x10 * i + 8) + 0x200;
                        uint code_len = BitConverter.ToUInt32(DecExe, 0x10 * i + 0xC);
                        for (int j = 0; j < code_len; j++)
                        {
                            DecExe[code_ofs + j] = (byte)(EncExe[code_ofs + j] ^ Exe_Alt[code_ofs + j]);
                        }
                    }
                }
                if (!ExeFS.validate(DecExe))
                {
                    CTR_ROM.updateTB(RTB_Progress, "Provided ExeFS or ExeFS Xorpad is invalid.");
                    return false;
                }
                byte[] EncExh = new byte[0x800];
                ROM.Seek(ExhOffset, SeekOrigin.Begin);
                ROM.Read(EncExh, 0, EncExh.Length);
                byte[] ExhPad = File.ReadAllBytes(Ex_Pad);
                byte[] DecExh = new byte[0x800];
                for (int i = 0; i < 0x800; i++)
                    DecExh[i] = (byte)(EncExh[i] ^ ExhPad[i]);
                byte[] ExhCalcHash = (new SHA256Managed()).ComputeHash(DecExh, 0, 0x400);
                if (!ExhCalcHash.SequenceEqual(ExheaderHash))
                {
                    CTR_ROM.updateTB(RTB_Progress, "Provided Exheader or Exheader Xorpad is invalid.");
                    return false;
                }
                // We're all valid.
                CTR_ROM.updateTB(RTB_Progress, "Validated. Extracting ROM contents to temporary directory...");
                ExeFS.get(DecExe, TmpDir + "exe" + Path.DirectorySeparatorChar);
                File.WriteAllBytes(TmpDir + "exhdr_dec.bin", DecExh);
                ROM.Seek(RomOffset, SeekOrigin.Begin);
                bool extra = (RomLen % 0x2000 != 0);
                PB_Show.Invoke(new Action(() =>
                {
                    PB_Show.Minimum = PB_Show.Value = 0;
                    PB_Show.Maximum = (int)(RomLen / 0x2000) + (extra ? 1 : 0);
                    PB_Show.Step = 1;
                }));
                using (var ROMPAD = File.OpenRead(Rom_Pad))
                {
                    using (var OUTROM = File.Open(TmpDir + "rom.bin", FileMode.CreateNew))
                    {
                        int BUF_SIZE = 0x400000;
                        byte[] buf = new byte[BUF_SIZE];
                        byte[] bufpad = new byte[BUF_SIZE];
                        int rounds = (int)(RomLen / 0x2000) + (extra ? 1 : 0);
                        for (uint i = 0; i < rounds; i++)
                        {
                            if (extra && i == rounds - 1)
                            {
                                BUF_SIZE = (int)((RomLen % 0x2000) * 0x200);
                            }
                            ROMPAD.Read(bufpad, 0,BUF_SIZE);
                            ROM.Read(buf, 0, BUF_SIZE);
                            for (int j = 0; j < BUF_SIZE; j++)
                                buf[j] = (byte)(buf[j] ^ bufpad[j]);
                            OUTROM.Write(buf, 0, BUF_SIZE);
                            PB_Show.Invoke(new Action(() =>
                            {
                                PB_Show.PerformStep();
                            }));
                        }
                    }
                }
                ROM.Seek(LogoOffset, SeekOrigin.Begin);
                ROM.Read(Logo, 0, Logo.Length);
                ROM.Seek(0x128, SeekOrigin.Begin);
                uint Manual_OFS = (uint)(0x200 * (ROM.ReadByte() | (ROM.ReadByte() << 8) | (ROM.ReadByte() << 16) | (ROM.ReadByte() << 24)));
                uint Manual_Len = (uint)((ROM.ReadByte() | (ROM.ReadByte() << 8) | (ROM.ReadByte() << 16) | (ROM.ReadByte() << 24)));
                ROM.Seek(Manual_OFS, SeekOrigin.Begin);
                using (var OUTMAN = File.Open(TmpDir + "Manual.ncch", FileMode.CreateNew))
                {
                    int BUF_SIZE = 0x400000;
                    byte[] buf = new byte[BUF_SIZE];
                    bool mextra = (Manual_Len % 0x2000 != 0);
                    int rounds = (int)(Manual_Len / 0x2000) + (mextra ? 1 : 0);
                    for (uint i = 0; i < rounds; i++)
                    {
                        if (extra && i == rounds - 1)
                        {
                            BUF_SIZE = (int)((Manual_Len % 0x2000) * 0x200);
                        }
                        ROM.Read(buf, 0, BUF_SIZE);
                        OUTMAN.Write(buf, 0, BUF_SIZE);
                    }
                }
                CTR_ROM.updateTB(RTB_Progress, "Extracting RomFS to Temporary Folder...");
                RomFS.ExtractRomFS(TmpDir + "rom" + Path.DirectorySeparatorChar, TmpDir + "rom.bin", RTB_Progress, PB_Show, ProgressLabel);
                try
                {
                    File.Delete(TmpDir + "rom.bin");
                }
                catch (IOException ex)
                { }
                CTR_ROM.updateTB(RTB_Progress, "ROM is validated and extracted.");
            }

            return true;
        }

        private void SetEnabled(bool en, params Control[] controls)
        {
            foreach (Control c in controls)
            {
                if (c.InvokeRequired)
                {
                    c.Invoke(new Action(() => c.Enabled = en));
                }
                else
                    c.Enabled = en;
            }
        }

        private void SetVisible(bool vi, params Control[] controls)
        {
            foreach (Control c in controls)
            {
                if (c.InvokeRequired)
                {
                    c.Invoke(new Action(() => c.Visible = vi));
                }
                else
                    c.Visible = vi;
            }
        }

        private void B_3DS_Click(object sender, EventArgs e)
        {
            if (threads > 0) { Alert("Please wait for all operations to finish first."); return; }

            string EXE_DIR = TmpDir + "exe";
            string ROMFS_DIR = TmpDir + "rom";
            string PATCH_DIR = TB_PatchDir.Text;
            string EXHEADER_PATH = TmpDir + "exhdr_dec.bin";
            string SERIAL_TEXT = Serial;
            string SAVE_PATH = Path.GetDirectoryName(TB_ROM.Text) + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(TB_ROM.Text) + "_Patched.3ds";
            byte[] Manual = File.ReadAllBytes(TmpDir + "Manual.ncch");
            new Thread(() =>
            {
                threads++;
                CTR_ROM.updateTB(RTB_Progress, "Patching Rom...");
                Patch(EXE_DIR, ROMFS_DIR, PATCH_DIR);
                // CTR_ROM.updateTB(RTB_Progress, "Patching Icon to be Region-Free...");
                // DoIconPatches(TmpDir + "exe" + Path.DirectorySeparatorChar + "icon.bin");
                CTR_ROM.updateTB(RTB_Progress, "Patching Exheader to allow lower firmwares...");
                DoExheaderPatches(EXHEADER_PATH);
                CTR_ROM.buildROM(Card2, Logo, EXE_DIR, ROMFS_DIR, EXHEADER_PATH, SERIAL_TEXT, SAVE_PATH, Manual, PB_Show, RTB_Progress);
                threads--;
            }).Start();
        }

        private void B_CIA_Click(object sender, EventArgs e)
        {
            if (threads > 0) { Alert("Please wait for all operations to finish first."); return; }

            string EXE_DIR = TmpDir + "exe";
            string ROMFS_DIR = TmpDir + "rom";
            string PATCH_DIR = TB_PatchDir.Text;
            string EXHEADER_PATH = TmpDir + "exhdr_dec.bin";
            string SERIAL_TEXT = Serial;
            string SAVE_PATH = Path.GetDirectoryName(TB_ROM.Text) + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(TB_ROM.Text) + "_Patched.cia";
            byte[] Manual = File.ReadAllBytes(TmpDir + "Manual.ncch");
            new Thread(() =>
            {
                threads++;
                CTR_ROM.updateTB(RTB_Progress, "Patching Rom...");
                Patch(EXE_DIR, ROMFS_DIR, PATCH_DIR);
                // CTR_ROM.updateTB(RTB_Progress, "Patching Icon to be Region-Free...");
                // DoIconPatches(TmpDir + "exe" + Path.DirectorySeparatorChar + "icon.bin");
                CTR_ROM.updateTB(RTB_Progress, "Patching Exheader to allow lower firmwares/SD play...");
                DoExheaderPatches(EXHEADER_PATH);
                CTR_ROM.buildCIA(Logo, EXE_DIR, ROMFS_DIR, EXHEADER_PATH, SERIAL_TEXT, SAVE_PATH, Manual, PB_Show, RTB_Progress);
                threads--;
            }).Start();
        }

        private void DoExheaderPatches(string EXHDR_PATH) // Allow SD usage + play from lower firmwares
        {
            byte[] EXHDR = File.ReadAllBytes(EXHDR_PATH);
            EXHDR[0xD] |= 0x2; // SDApplication bitflag
            Array.Copy(BitConverter.GetBytes((ushort)0x221), 0, EXHDR, 0x39C, 2); // Patch ARM11 Kernel Descriptor Firmware Version
            Array.Copy(BitConverter.GetBytes((ushort)0x221), 0, EXHDR, 0x79C, 2); // Patch ARM11 Kernel Descriptor Firmware Version in Limitation Copy
            File.WriteAllBytes(EXHDR_PATH, EXHDR);
        }

        private void DoIconPatches(string ICON_PATH) // Region-Free + Game Name
        {
            byte[] SMDH = File.ReadAllBytes(ICON_PATH);
            byte[] NameDesc = new byte[0x200];
            Array.Copy(SMDH, 0x8, NameDesc, 0x0, 0x200);
            for (int i = 0; i < 6; i++) // Copy name to all languages.
            {
                Array.Copy(NameDesc, 0, SMDH, 0x8 + i * NameDesc.Length, NameDesc.Length);
            }
            for (int i = 0; i < 3; i++) // Copy name to all languages.
            {
                Array.Copy(NameDesc, 0, SMDH, 0x1008 + i * NameDesc.Length, NameDesc.Length);
            }
            Array.Copy(BitConverter.GetBytes(0x7FFFFFFF), 0, SMDH, 0x2018, 4); // Make it Region Free
            File.WriteAllBytes(ICON_PATH, SMDH);
        }

        private void Patch(string EXE_DIR, string ROM_DIR, string PATCH_DIR)
        {
            string PATCH_EXE = PATCH_DIR + "exe" + Path.DirectorySeparatorChar;
            string PATCH_ROM = PATCH_DIR + "rom" + Path.DirectorySeparatorChar;
            CopyAll((new DirectoryInfo(PATCH_EXE)), (new DirectoryInfo(EXE_DIR)));
            CopyAll((new DirectoryInfo(PATCH_ROM)), (new DirectoryInfo(ROM_DIR)));
        }

        public static void DeleteDirectory(string target_dir)
        {
            string[] files = Directory.GetFiles(target_dir);
            string[] dirs = Directory.GetDirectories(target_dir);

            foreach (string file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            foreach (string dir in dirs)
            {
                DeleteDirectory(dir);
            }

            Directory.Delete(target_dir, false);
        }

        public static void CopyAll(DirectoryInfo source, DirectoryInfo target)
        {
            if (source.FullName.ToLower() == target.FullName.ToLower())
            {
                return;
            }

            // Check if the target directory exists, if not, create it.
            if (Directory.Exists(target.FullName) == false)
            {
                Directory.CreateDirectory(target.FullName);
            }

            // Copy each file into it's new directory.
            foreach (FileInfo fi in source.GetFiles())
            {
                fi.CopyTo(Path.Combine(target.ToString(), fi.Name), true);
            }

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir =
                    target.CreateSubdirectory(diSourceSubDir.Name);
                CopyAll(diSourceSubDir, nextTargetSubDir);
            }
        }
    }
}
