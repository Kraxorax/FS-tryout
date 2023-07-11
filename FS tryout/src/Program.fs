open TheGame
open System.Diagnostics

[<EntryPoint>]
let main args =
    // Start game
    using (new Game(800, 800, "HEWEEJ YOOO!")) (fun game -> game.Run())
    Debug.WriteLine "Program's done. Gone!"
    0 // Exit code
