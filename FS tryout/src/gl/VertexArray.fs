namespace TheGameGL

open System
open OpenTK.Graphics.OpenGL

type VertexArray(vertBuffer: VertexBuffer) as it =
    let mutable disposed = false

    let BufferID = GL.GenVertexArray()

    let vertexBuffer = vertBuffer

    do
        GL.BindVertexArray(BufferID)
        GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBuffer.VertexBufferID)

        let vertexSizeInBytes = vertexBuffer.VertexInfo.SizeInBytes
        let vertexAttribs = vertexBuffer.VertexInfo.VertexAttribs

        for va in vertexAttribs do
            GL.VertexAttribPointer(
                va.Index,
                va.ComponentCount,
                VertexAttribPointerType.Float,
                false,
                vertexSizeInBytes,
                va.Offset
            )
            GL.EnableVertexAttribArray(va.Index)
        GL.BindVertexArray(0)

    member this.ID with get() = BufferID
    
    member this.VertexBuffer with get() = vertexBuffer

    member this.Cleanup =
        if not disposed then
            GL.DeleteBuffer(BufferID)
            disposed <- true

    interface IDisposable with
        member it.Dispose() = 
            it.Cleanup
            GC.SuppressFinalize(it)

    override this.Finalize() = it.Cleanup
