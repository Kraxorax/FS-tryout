namespace TheGame

open System
open OpenTK.Windowing.Desktop
open OpenTK.Graphics.OpenGL
open OpenTK.Mathematics
open OpenTK.Windowing.Common
open TheGameGL
open GoL

type Game(width, height, title) as this =
    inherit
        GameWindow(
            GameWindowSettings.Default,
            new NativeWindowSettings(
                Title = title,
                Size = Vector2i(width, height),
                WindowBorder = WindowBorder.Fixed,
                StartVisible = false,
                StartFocused = true,
                API = ContextAPI.OpenGL,
                Profile = ContextProfile.Core,
                APIVersion = new Version(3, 3)
            )
        )

    let mutable vertexCount = 0

    let mutable indexCount = 0

    let mutable vertexBuffer: Option<VertexBuffer> = Option.None

    let mutable indexBuffer: Option<IndexBuffer> = Option.None

    let mutable vertexArray: Option<VertexArray> = Option.None

    let mutable shaderProgram: ShaderProgram = new ShaderProgram()

    let mutable colorFactor = 1.0f
    let mutable deltaColorFactor = 1f / 9240f

    let mutable gol: Option<GoLGame> = Option.None

    let cols = 50
    let rows = 50
    let rektCount = cols * rows
    let rektIndices = [| 0; 1; 2; 2; 3; 0 |]


    let mutable vertices = Array.zeroCreate (rektCount * Rekt.numVertices)
    let mutable indices = Array.zeroCreate (rektCount * Rekt.numIndices)

    let updateBuffers(squares: Rekt[]) =
        vertexCount <- 0

        for sqrInd in [0 .. squares.Length-1] do
            let square = squares.[sqrInd]
            for vrtInd in [0 .. square.vertices.Length-1] do
                vertices[vertexCount + vrtInd] <- square.vertices[vrtInd]
            vertexCount <- vertexCount + Rekt.numVertices
        vertexBuffer.Value.SetData(vertices, vertices.Length)

    let doUpdate() =
        let squares = gol.Value.Board.MapToArray (fun cell -> cell.GetRekt() |> Rekt)
        updateBuffers(squares)


    do  this.CenterWindow()
        gol <- new GoLGame(cols, rows, width, height, doUpdate) |> Option.Some
        let rand = new Random()
        let boxCount = 1000
        let ssquares =
            [| for i in [0 .. boxCount-1] ->
                let w = rand.Next(32, 128) |> float32
                let h = rand.Next(32, 128) |> float32
                let x = rand.Next(0, (float32 this.Size.X - w) |> int) |> float32
                let y = rand.Next(0, (float32 this.Size.Y - h) |> int) |> float32

                let r = rand.NextDouble() |> float32
                let g = rand.NextDouble() |> float32
                let b = rand.NextDouble() |> float32

                Rekt(x, y, w, h, r, g, b)
            |]

        let squares = gol.Value.Board.MapToArray (fun cell -> cell.GetRekt() |> Rekt)

        indexCount <- 0
        vertexCount <- 0

        for sqrInd in [0 .. squares.Length-1] do
            let square = squares.[sqrInd]
            for vrtInd in [0 .. square.vertices.Length-1] do
                vertices[vertexCount + vrtInd] <- square.vertices[vrtInd]
            for indInd in [0 .. rektIndices.Length-1] do
                indices[indexCount + sqrInd + indInd] <- rektIndices[indInd] + vertexCount

            indexCount <- indexCount + Rekt.numIndices - 1
            vertexCount <- vertexCount + Rekt.numVertices

        vertexBuffer <- new VertexBuffer(VertexPosCol.VertexInfo, vertices.Length, true) |> Option.Some
        vertexBuffer.Value.SetData(vertices, vertices.Length)

        indexBuffer <- new IndexBuffer(indices.Length, true) |> Option.Some
        indexBuffer.Value.SetData(indices, indices.Length)

        vertexArray <- new VertexArray(vertexBuffer.Value) |> Option.Some





    override this.OnLoad() =
        this.IsVisible <- true
        GL.ClearColor(Color4.Aquamarine)

        let mutable viewport = [| 0; 0; 0; 0 |]
        GL.GetInteger(GetPName.Viewport, viewport)
        shaderProgram.SetUniform("viewportSize", viewport.[2], viewport.[3])

        shaderProgram.SetUniform("colorFactor", colorFactor)

        gol.Value.Start()

        base.OnLoad()


    override this.OnUnload() =
        vertexBuffer.Value.Cleanup
        indexBuffer.Value.Cleanup
        vertexArray.Value.Cleanup

        base.OnUnload()


    override this.OnRenderFrame(e) =
        GL.Clear(ClearBufferMask.ColorBufferBit)

        GL.UseProgram(shaderProgram.ID)

        GL.BindVertexArray(vertexArray.Value.ID)

        GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBuffer.Value.ID)

        GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0)

        this.SwapBuffers()
        base.OnRenderFrame(e)


    override this.OnUpdateFrame(e) = 
        doUpdate()
        //colorFactor <- colorFactor + deltaColorFactor

        //if colorFactor > 1.0f then
        //    deltaColorFactor <- deltaColorFactor * -1.0f
        //else if colorFactor < 0.5f then
        //    deltaColorFactor <- deltaColorFactor * -1.0f


        //shaderProgram.SetUniform("colorFactor", colorFactor)

        base.OnUpdateFrame(e)
