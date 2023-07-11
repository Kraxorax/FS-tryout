namespace TheGameGL

open System
open OpenTK.Mathematics

[<Struct>]
type VertexAttrib =
    val Name: string
    val Index: int
    val ComponentCount: int
    val Offset: int
    
    new(name, index, componentCount, offset) =
        { Name = name
          Index = index
          ComponentCount = componentCount
          Offset = offset }

type VertexInfo =
    val Type: Type
    val SizeInBytes: int
    val VertexAttribs: VertexAttrib[]

    new(type', [<ParamArray>] ps: VertexAttrib[]) =
        { Type = type'
          SizeInBytes = ps |> Array.sumBy (fun p -> p.ComponentCount * sizeof<float32>)
          VertexAttribs = ps }

[<Struct>]
type VertexPosCol =
    val Position: Vector2
    val Color: Vector4

    static member VertexInfo: VertexInfo =
        VertexInfo(
            typeof<VertexPosCol>,
            [| VertexAttrib("aPos", 0, 2, 0)
               VertexAttrib("aColor", 1, 4, 2 * sizeof<float32>) |]
        )

    new(position, color) = { Position = position; Color = color }

    override this.ToString() =
        sprintf "Position: %A, Color: %A" this.Position this.Color

[<Struct>]
type VertexPosTex =
    val Position: Vector2
    val TexCoord: Vector2

    static member VertexInfo: VertexInfo =
        VertexInfo(
            typeof<VertexPosTex>,
            [| VertexAttrib("aPos", 0, 2, 0)
               VertexAttrib("aTexCoord", 1, 2, 2 * sizeof<float32>) |]
        )

    new(position, texCoord) =
        { Position = position
          TexCoord = texCoord }

    override this.ToString() =
        sprintf "Position: %A, TexCoord: %A" this.Position this.TexCoord
