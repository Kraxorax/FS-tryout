module GoL

open System.Diagnostics
open OpenTK.Mathematics
open Matrix

type CellState = Dead | Alive

type CellViewConfig = { TickInterval: float; AliveColor: Vector4; DeadColor: Vector4; }

let conf = {
    TickInterval = 100.0;
    AliveColor = Vector4(1.0f, 1.0f, 1.0f, 1.0f);
    DeadColor = Vector4(0.0f, 0.0f, 0.0f, 1.0f);
}

type Cell (x: int, y: int, w: float32, h: float32, cellState:CellState) =
    let mutable CellState:CellState = cellState
    let mutable Color:Vector4 = if cellState = Alive then conf.AliveColor else conf.DeadColor

    member this.GetRekt() = (float32 x * w, float32 y * h, w, h, Color.X, Color.Y, Color.Z)

    member this.State
        with get() = CellState
        and set(value) = do
            CellState <- value
            Color <- if value = Alive then conf.AliveColor else conf.DeadColor


type GoLBoard = Matrix<Cell>

let createBoardWith(w: int, h: int, scrPxW: int, scrPxH: int, f: int*int->CellState): GoLBoard =
    let cellW = float32 scrPxW / float32 w
    let cellH = float32 scrPxH / float32 h

    let board = new GoLBoard(w, h, fun x y -> new Cell(x, y, cellW, cellH, f(x, y)))
    board

let rule (board: GoLBoard) (x: int) (y: int): CellState =
    let cell = board.Item(x, y)
    let numAliveNeighbours = board.Neighbours(x, y) |> Array.filter (fun cell -> cell.State = Alive) |> Array.length
    match cell.State with
    | Dead -> if (numAliveNeighbours = 3) then Alive else Dead
    | Alive -> if (numAliveNeighbours = 2 || numAliveNeighbours = 3) then Alive else Dead


type GoLGame(cols, rows, width, height, update) as it =
    let randomGenerator (_, _) = if System.Random().Next(0, 2) = 0 then Dead else Alive
    let deadGenerator (_, _) = Dead
    let mutable leftBoard = createBoardWith(cols, rows, width, height, randomGenerator)
    let mutable rightBoard = createBoardWith(cols, rows, width, height, deadGenerator)
    let mutable readFromLeft = true
    let tickTimer = new System.Timers.Timer(conf.TickInterval)
    let mutable tickCount = 0

    let runRule () =
        let readBoard = if readFromLeft then leftBoard else rightBoard
        let writeBoard = if readFromLeft then rightBoard else leftBoard
        for x in [0 .. readBoard.X-1] do
            for y in [0 .. readBoard.Y-1] do
                writeBoard.Item(x, y).State <- rule readBoard x y
        readFromLeft <- not readFromLeft

    let onTick (_:System.Timers.ElapsedEventArgs) =
        runRule()
        tickCount <- tickCount + 1
        Debug.WriteLine (sprintf "Tick: %d" tickCount)

    do
        tickTimer.Elapsed.Add(onTick)

    member this.Board with get() = if readFromLeft then leftBoard else rightBoard

    member this.Start() = tickTimer.Start()


