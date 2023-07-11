namespace TheGameGL

open System
open OpenTK.Graphics.OpenGL

type IndexBuffer(indexCount: int, isStatc: bool) as it =
    let MinIndexCount = 1
    let MaxIndexCount = Int64.MaxValue
    let mutable disposed = false

    let BufferID = GL.GenBuffer() in member this.ID with get() = BufferID

    [<DefaultValue>]
    val mutable IndexCount: int
    [<DefaultValue>]
    val mutable IsStatic: bool

    do
        if (indexCount < MinIndexCount || MaxIndexCount < indexCount)
        then raise (ArgumentOutOfRangeException(nameof indexCount))

        it.IndexCount <- indexCount

        GL.BindBuffer(BufferTarget.ElementArrayBuffer, BufferID)

        let bufferUsageHint = if it.IsStatic then BufferUsageHint.StaticDraw else BufferUsageHint.StreamDraw

        GL.BufferData(
            BufferTarget.ElementArrayBuffer,
            it.IndexCount * sizeof<int>,
            IntPtr.Zero,
            bufferUsageHint
        )

        GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0)

    member this.SetData(data: int[], count: int) =
        if (data |> isNull) then raise (ArgumentNullException(nameof data))
        if (data.Length <= 0) then raise (ArgumentOutOfRangeException(nameof data))
        if (count <= 0 || count > this.IndexCount || count > data.Length) then raise (ArgumentOutOfRangeException(nameof data))
        
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, BufferID)
        GL.BufferSubData(BufferTarget.ElementArrayBuffer, IntPtr.Zero, count * sizeof<int>, data)
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0)

    member this.Cleanup =
        if not disposed then
            GL.DeleteBuffer(BufferID)
            disposed <- true

    interface IDisposable with
        member it.Dispose() =
            it.Cleanup
            GC.SuppressFinalize(it)
            

    override this.Finalize() = it.Cleanup