using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Battle_Tank_Game;

class Program
{
    // Array sprite tank (index 0 diganti kosong supaya aman)
    public static string[] Tank =
    {
        "     \n     \n     \n", // 0 = kosong
        // Up
        @" █ " + "\n" +
        @"|_|" + "\n" +
        @"[ooo]" + "\n",
        // Down
        @" _ " + "\n" +
        @"|█|" + "\n" +
        @"[ooo]" + "\n",
        // Left
        @"  __ " + "\n" +
        @"█|__|" + "\n" +
        @"[ooo]" + "\n",
        // Right
        @" __  " + "\n" +
        @"|__|█" + "\n" +
        @"[ooo]" + "\n",
    };

    // Tank exploding
    public static string[] TankExploding =
    {
        @" _ " + "\n" +
        @"|_|" + "\n" +
        @"[ooo]" + "\n",
        @"█████" + "\n" +
        @"█████" + "\n" +
        @"█████" + "\n",
        @"     " + "\n" +
        @"     " + "\n" +
        @"     " + "\n",
    };

    // Bullet char
    public static char[] Bullet =
    {
        ' ', // 0 = kosong
        '^', // Up
        'v', // Down
        '<', // Left
        '>', // Right
    };

    public static string Map =
        @"╔═════════════════════════════════════════════════════════════════════════╗" + "\n" +
        @"║                                                                         ║" + "\n" +
        @"║                                                                         ║" + "\n" +
        @"║                                                                         ║" + "\n" +
        @"║                                    ║                                    ║" + "\n" +
        @"║                                    ║                                    ║" + "\n" +
        @"║                                    ║                                    ║" + "\n" +
        @"║                                                                         ║" + "\n" +
        @"║                                                                         ║" + "\n" +
        @"║                                                                         ║" + "\n" +
        @"║                                                                         ║" + "\n" +
        @"║                                                                         ║" + "\n" +
        @"║                                                                         ║" + "\n" +
        @"║     ═════                                                     ═════     ║" + "\n" +
        @"║                                                                         ║" + "\n" +
        @"║                                                                         ║" + "\n" +
        @"║                                                                         ║" + "\n" +
        @"║                                                                         ║" + "\n" +
        @"║                                                                         ║" + "\n" +
        @"║                                                                         ║" + "\n" +
        @"║                                                                         ║" + "\n" +
        @"║                                    ║                                    ║" + "\n" +
        @"║                                    ║                                    ║" + "\n" +
        @"║                                    ║                                    ║" + "\n" +
        @"║                                                                         ║" + "\n" +
        @"║                                                                         ║" + "\n" +
        @"║                                                                         ║" + "\n" +
        @"╚═════════════════════════════════════════════════════════════════════════╝" + "\n";

    // Grid peta untuk deteksi collision dan restorasi karakter peta
    public static char[,] MapGrid = null!;
    public static int MapRows;
    public static int MapCols;

    static void Main(string[] args)
    {
        // FIX: Parse peta menjadi grid 2D agar bisa deteksi tabrakan dinding
        // dan mengembalikan karakter peta yang tertimpa sprite tank/peluru
        var mapLines = Map.Split('\n');
        MapRows = mapLines.Length;
        MapCols = 75; // Lebar peta tetap 75 karakter
        MapGrid = new char[MapRows, MapCols];
        for (int r = 0; r < MapRows; r++)
            for (int c = 0; c < mapLines[r].Length && c < MapCols; c++)
                MapGrid[r, c] = mapLines[r][c];

        var Tanks = new List<Tank>();
        var AllTanks = new List<Tank>();
        var Player = new Tank() { X = 8, Y = 5, IsPlayer = true };

        Tanks.Add(Player);
        Tanks.Add(new Tank() { X = 8, Y = 21 });
        Tanks.Add(new Tank() { X = 66, Y = 5 });
        Tanks.Add(new Tank() { X = 66, Y = 21 });
        AllTanks.AddRange(Tanks);

        Console.CursorVisible = false;
        Console.OutputEncoding = Encoding.UTF8;
        Console.Clear();
        Console.SetCursorPosition(0, 0);
        Console.Write(Map);
        Console.WriteLine();
        Console.WriteLine("Use (W, A, S, D) keys to move and the arrow keys to shoot,");

        while (Tanks.Contains(Player) && Tanks.Count > 1)
        {
            foreach (var tank in Tanks.ToArray())
            {
                tank.ClearPrevious();

                if (tank.IsExploding)
                {
                    tank.ExplodingFrame++;
                    tank.DrawExplosion();
                    continue;
                }

                if (tank.IsPlayer)
                {
                    if (Console.KeyAvailable)
                    {
                        var key = Console.ReadKey(true).Key;
                        switch (key)
                        {
                            case ConsoleKey.W: tank.Direction = Direction.Up; break;
                            case ConsoleKey.S: tank.Direction = Direction.Down; break;
                            case ConsoleKey.A: tank.Direction = Direction.Left; break;
                            case ConsoleKey.D: tank.Direction = Direction.Right; break;

                            case ConsoleKey.UpArrow: tank.Shoot(Direction.Up); break;
                            case ConsoleKey.DownArrow: tank.Shoot(Direction.Down); break;
                            case ConsoleKey.LeftArrow: tank.Shoot(Direction.Left); break;
                            case ConsoleKey.RightArrow: tank.Shoot(Direction.Right); break;

                            case ConsoleKey.Escape:
                                Console.Clear();
                                Console.WriteLine("Game closed.");
                                return;
                        }
                    }
                }
                else
                {
                    if (tank.AICooldown-- <= 0)
                    {
                        tank.Direction = (Direction)(Random.Shared.Next(1, 5));
                        tank.AICooldown = 8;
                    }
                    tank.ShootIfPossible(Player);
                }

                // FIX: Teruskan daftar tank agar bisa deteksi tabrakan antar tank
                tank.Move(Tanks);
                tank.Draw();
            }

            foreach (var tank in AllTanks)
            {
                tank.UpdateBullet(Tanks);
            }

            // FIX: Bersihkan visual tank yang sudah selesai meledak sebelum dihapus dari daftar
            // Tanpa ini, gambar ledakan akan tertinggal permanen di layar
            foreach (var t in Tanks.ToArray())
            {
                if (t.IsExploding && t.ExplodingFrame > 10)
                    t.ClearFinal();
            }
            Tanks.RemoveAll(t => t.IsExploding && t.ExplodingFrame > 10);

            Thread.Sleep(50);
        }

        Console.SetCursorPosition(0, 33);
        Console.Write(Tanks.Contains(Player) ? "You Win." : "You Lose.");
        Console.ReadLine();
    }
}

enum Direction
{
    Null = 0,
    Up = 1,
    Down = 2,
    Left = 3,
    Right = 4,
}

class Tank
{
    public bool IsPlayer;
    public int Health = 4;
    public int X;
    public int Y;
    public Direction Direction = Direction.Down;
    public Bullet? Bullet;
    public int ExplodingFrame;
    public int AICooldown;
    public bool IsExploding => ExplodingFrame > 0;

    private int prevX;
    private int prevY;

    // FIX: Tambah parameter List<Tank> untuk deteksi tabrakan dengan tank lain
    public void Move(List<Tank> tanks)
    {
        prevX = X;
        prevY = Y;

        int newX = X, newY = Y;
        switch (Direction)
        {
            case Direction.Up: newY--; break;
            case Direction.Down: newY++; break;
            case Direction.Left: newX--; break;
            case Direction.Right: newX++; break;
        }

        // FIX: Cek tabrakan dengan dinding peta dan batas layar
        if (WouldCollideWithMap(newX, newY)) return;

        // FIX: Cek tabrakan dengan tank lain agar tidak bisa overlap
        foreach (var other in tanks)
        {
            if (other == this || other.IsExploding) continue;
            if (Math.Abs(other.X - newX) < 4 && Math.Abs(other.Y - newY) < 2)
                return;
        }

        X = newX;
        Y = newY;
    }

    // FIX: Cek apakah posisi baru akan menabrak dinding atau batas peta
    // Tank berukuran 5 lebar x 3 tinggi, berpusat di (X, Y)
    private bool WouldCollideWithMap(int newX, int newY)
    {
        for (int dy = -1; dy <= 1; dy++)
        {
            for (int dx = -2; dx <= 2; dx++)
            {
                int r = newY + dy;
                int c = newX + dx;
                // Di luar batas array grid = tabrakan
                if (r < 0 || r >= Program.MapRows || c < 0 || c >= Program.MapCols)
                    return true;
                char ch = Program.MapGrid[r, c];
                // Karakter non-spasi (dinding, border) = tabrakan
                if (ch != ' ' && ch != '\0')
                    return true;
            }
        }
        return false;
    }

    public void Shoot(Direction dir)
    {
        if (Bullet != null) return;
        // FIX: Simpan referensi pemilik untuk mencegah peluru mengenai tank yang menembak
        Bullet = new Bullet { X = X, Y = Y, Direction = dir, Owner = this };
    }

    public void ShootIfPossible(Tank player)
    {
        if (Bullet != null) return;

        Direction shootDir = Math.Abs(player.X - X) > Math.Abs(player.Y - Y)
            ? (X < player.X ? Direction.Right : Direction.Left)
            : (Y < player.Y ? Direction.Down : Direction.Up);

        Shoot(shootDir);
    }

    public void Draw()
    {
        // FIX: Console.Write dengan "\n" di dalamnya memindahkan kursor ke kolom 0,
        // bukan kembali ke posisi X. Gambar setiap baris secara terpisah dengan
        // SetCursorPosition agar semua baris tampil di posisi yang benar.
        var lines = Program.Tank[Math.Max(1, (int)Direction)].Split('\n');
        for (int i = 0; i < 3; i++)
        {
            Console.SetCursorPosition(X - 2, Y - 1 + i);
            string line = (i < lines.Length) ? lines[i] : "";
            // Tulis tepat 5 karakter agar baris sebelumnya selalu tertimpa bersih
            Console.Write((line + "     ").Substring(0, 5));
        }
    }

    public void DrawExplosion()
    {
        // FIX: Animasi ledakan bertahap menggunakan semua 3 frame yang tersedia:
        // frame awal  → outline tank (sekarat)
        // frame tengah → blok padat (ledakan)
        // frame akhir  → kosong (tank hilang, tapi ClearFinal yang handle bersih)
        int frameIndex;
        if (ExplodingFrame <= 3) frameIndex = 0;       // Tank outline (sekarat)
        else if (ExplodingFrame <= 8) frameIndex = 1;  // Blok ledakan
        else frameIndex = 2;                           // Kosong

        // FIX: Sama seperti Draw(), setiap baris diposisikan manual
        var lines = Program.TankExploding[frameIndex].Split('\n');
        for (int i = 0; i < 3; i++)
        {
            Console.SetCursorPosition(X - 2, Y - 1 + i);
            string line = (i < lines.Length) ? lines[i] : "";
            Console.Write((line + "     ").Substring(0, 5));
        }
    }

    public void ClearPrevious()
    {
        if (prevX == 0 && prevY == 0) return;

        // FIX: Dua perbaikan sekaligus:
        // 1. SetCursorPosition dipanggil per baris agar tidak kembali ke kolom 0
        // 2. Karakter peta (dinding, border) dikembalikan dari MapGrid, bukan ditulis spasi
        for (int dy = -1; dy <= 1; dy++)
        {
            Console.SetCursorPosition(prevX - 2, prevY + dy);
            for (int dx = -2; dx <= 2; dx++)
            {
                int r = prevY + dy;
                int c = prevX + dx;
                char mapChar = (r >= 0 && r < Program.MapRows && c >= 0 && c < Program.MapCols)
                    ? Program.MapGrid[r, c]
                    : ' ';
                Console.Write(mapChar == '\0' ? ' ' : mapChar);
            }
        }
    }

    // FIX: Bersihkan visual akhir tank setelah animasi ledakan selesai
    // ClearPrevious tidak bisa dipakai karena prevX/prevY sama dengan X/Y (Move tidak dipanggil saat meledak)
    public void ClearFinal()
    {
        for (int dy = -1; dy <= 1; dy++)
        {
            Console.SetCursorPosition(X - 2, Y + dy);
            for (int dx = -2; dx <= 2; dx++)
            {
                int r = Y + dy;
                int c = X + dx;
                char mapChar = (r >= 0 && r < Program.MapRows && c >= 0 && c < Program.MapCols)
                    ? Program.MapGrid[r, c]
                    : ' ';
                Console.Write(mapChar == '\0' ? ' ' : mapChar);
            }
        }
    }

    public void UpdateBullet(List<Tank> Tanks)
    {
        if (Bullet == null) return;

        // FIX: Kembalikan karakter peta di posisi peluru sebelumnya, bukan tulis spasi
        if (Bullet.X >= 0 && Bullet.Y >= 0 && Bullet.X < Program.MapCols && Bullet.Y < Program.MapRows)
        {
            char mapChar = Program.MapGrid[Bullet.Y, Bullet.X];
            Console.SetCursorPosition(Bullet.X, Bullet.Y);
            Console.Write(mapChar == '\0' ? ' ' : mapChar);
        }

        switch (Bullet.Direction)
        {
            case Direction.Up: Bullet.Y--; break;
            case Direction.Down: Bullet.Y++; break;
            case Direction.Left: Bullet.X--; break;
            case Direction.Right: Bullet.X++; break;
        }

        // FIX: Hentikan peluru jika keluar batas peta
        if (Bullet.X < 0 || Bullet.X >= Program.MapCols ||
            Bullet.Y < 0 || Bullet.Y >= Program.MapRows)
        {
            Bullet = null;
            return;
        }

        // FIX: Hentikan peluru jika mengenai dinding atau border peta
        char cell = Program.MapGrid[Bullet.Y, Bullet.X];
        if (cell != ' ' && cell != '\0')
        {
            Bullet = null;
            return;
        }

        foreach (var t in Tanks)
        {
            if (Bullet == null) break;
            // FIX: Lewati tank pemilik peluru agar tidak bisa mengenai diri sendiri
            if (t == Bullet.Owner) continue;
            if (Math.Abs(t.X - Bullet.X) < 2 && Math.Abs(t.Y - Bullet.Y) < 1)
            {
                t.Health--;
                if (t.Health <= 0) t.ExplodingFrame = 1;
                Bullet = null;
            }
        }

        if (Bullet != null)
        {
            Console.SetCursorPosition(Bullet.X, Bullet.Y);
            Console.Write(Program.Bullet[Math.Max(1, (int)Bullet.Direction)]);
        }
    }
}

class Bullet
{
    public int X;
    public int Y;
    public Direction Direction;
    // FIX: Tambah referensi pemilik untuk mencegah self-hit
    public Tank? Owner;
}