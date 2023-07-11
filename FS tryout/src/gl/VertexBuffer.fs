namespace TheGameGL

open System
open OpenTK.Graphics.OpenGL


type VertexBuffer(vi: VertexInfo, vertexCount: int, isStatic: bool) as it = 
    let MinVertexCount = 0
    let MaxVertexCount = Int64.MaxValue
    let mutable disposed = false

    [<DefaultValue>]
    val mutable VertexBufferID: int
    [<DefaultValue>]
    val mutable VertexInfo: VertexInfo
    [<DefaultValue>]
    val mutable VertexCount: int
    [<DefaultValue>]
    val mutable IsStatic: bool

    do
        if (vertexCount < MinVertexCount || MaxVertexCount < vertexCount)
        then raise (ArgumentOutOfRangeException(nameof vertexCount))
        
        it.VertexInfo <- vi
        it.VertexCount <- vertexCount
        it.IsStatic <- isStatic
        it.VertexBufferID <- GL.GenBuffer()
        
        let bufferUsageHint = if it.IsStatic then BufferUsageHint.StaticDraw else BufferUsageHint.StreamDraw
        let vertexSizeInBytes = VertexPosCol.VertexInfo.SizeInBytes

        GL.BindBuffer(BufferTarget.ArrayBuffer, it.VertexBufferID)
        GL.BufferData(
            BufferTarget.ArrayBuffer,
            it.VertexCount * it.VertexInfo.SizeInBytes,
            IntPtr.Zero,
            bufferUsageHint
        )
        // Unbind buffer
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0)

        
    member this.SetData<'T when 'T : struct and 'T :> ValueType and 'T : (new: unit -> 'T)>(data: 'T[], count: int) =
        if (typeof<'T> <> this.VertexInfo.Type) then raise (ArgumentException(nameof data))
        if (data |> isNull) then raise (ArgumentNullException(nameof data))
        if (data.Length <= 0) then raise (ArgumentOutOfRangeException(nameof data))
        if (count <= 0 || this.VertexCount < count || count > data.Length) then raise (ArgumentOutOfRangeException(nameof count))
        
        GL.BindBuffer(BufferTarget.ArrayBuffer, this.VertexBufferID)
        GL.BufferSubData(
            BufferTarget.ArrayBuffer,
            IntPtr.Zero,
            count * this.VertexInfo.SizeInBytes,
            data
        )
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0)
        

    member this.Cleanup =
        if not disposed then
            GL.DeleteBuffer(this.VertexBufferID)
            disposed <- true

    interface IDisposable with
        member it.Dispose() = 
            it.Cleanup
            GC.SuppressFinalize(it)
            

    override this.Finalize() = it.Cleanup
