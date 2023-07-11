namespace TheGameGL

open OpenTK.Mathematics

type Rekt =
    val vertices: VertexPosCol[]
    //static member indices = [| 0; 1; 2; 2; 3; 0 |]
    static member numVertices = 4
    static member numIndices = 6

    new(x, y, w, h, r, g, b) =
        {
            vertices = [|
                VertexPosCol(Vector2(x, y + h), Vector4(r, g, b, 1.0f))
                VertexPosCol(Vector2(x + w, y + h), Vector4(r, g, b, 1.0f))
                VertexPosCol(Vector2(x + w, y), Vector4(r, g, b, 1.0f))
                VertexPosCol(Vector2(x, y), Vector4(r, g, b, 1.0f))
            |]
        }

