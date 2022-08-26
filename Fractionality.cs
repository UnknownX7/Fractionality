using System;
using Dalamud.Game;
using Dalamud.Game.Gui;
using Dalamud.Plugin;

namespace Fractionality
{
    public class Fractionality : IDalamudPlugin
    {
        public string Name => "Fractionality";

        public Fractionality(ChatGui chatGui, SigScanner sigScanner)
        {
            _ = new DalamudApi() + sigScanner + chatGui;

            try
            {
                var ptr1000f = DalamudApi.SigScanner.GetStaticAddressFromSig("4C 8D 25 ?? ?? ?? ?? 4C 63 F1");
                var waitSyntax = DalamudApi.SigScanner.ScanModule("F3 0F 58 05 ?? ?? ?? ?? F3 48 0F 2C C0 69 C8");
                var waitCommand = DalamudApi.SigScanner.ScanModule("F3 0F 58 0D ?? ?? ?? ?? F3 48 0F 2C C1 69 C8");
                var newOffsetBytes1 = BitConverter.GetBytes((int)(ptr1000f.ToInt64() - (waitSyntax + 0x8).ToInt64()));
                var newOffsetBytes2 = BitConverter.GetBytes((int)(ptr1000f.ToInt64() - (waitCommand + 0x8).ToInt64()));

                var waitSyntaxFractionalReplacer = new Memory.Replacer(waitSyntax, new byte[] {
                    0xF3, 0x0F, 0x59, 0x05, newOffsetBytes1[0], newOffsetBytes1[1], newOffsetBytes1[2], newOffsetBytes1[3],
                    0xF3, 0x48, 0x0F, 0x2C, 0xC8,
                    0x90, 0x90, 0x90, 0x90, 0x90, 0x90,
                    0x90, 0x90, 0x90, 0x90, 0x90,
                    0x90, 0x90,
                    0x90, 0x90, 0x90
                });
                waitSyntaxFractionalReplacer.Enable();

                var waitCommandFractionalReplacer = new Memory.Replacer(waitCommand, new byte[] {
                    0xF3, 0x0F, 0x59, 0x0D, newOffsetBytes2[0], newOffsetBytes2[1], newOffsetBytes2[2], newOffsetBytes2[3],
                    0xF3, 0x48, 0x0F, 0x2C, 0xC9,
                    0x90, 0x90, 0x90, 0x90, 0x90, 0x90,
                    0x89, 0x4B, 0x58,
                    0x90, 0x90, 0x90, 0x90, 0x90, 0x90,
                    0xEB // 0x1F
                });
                waitCommandFractionalReplacer.Enable();
            }
            catch { PrintError("Failed to load!"); }
        }

        public static void PrintEcho(string message) => DalamudApi.ChatGui.Print($"[Fractionality] {message}");
        public static void PrintError(string message) => DalamudApi.ChatGui.PrintError($"[Fractionality] {message}");

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) return;
            Memory.Dispose();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
