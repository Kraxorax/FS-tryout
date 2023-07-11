namespace TheGameGL

open System
open System.IO
open OpenTK.Graphics.OpenGL

[<Struct>]
type ShaderUniform =
    val Name: string
    val Location: int
    val Type: ActiveUniformType

    new(name, location, type') =
        { Name = name
          Location = location
          Type = type' }

[<Struct>]
type ShaderAttribute =
    val Name: string
    val Location: int
    val Type: ActiveAttribType
    
    new(name, location, type') =
        { Name = name
          Location = location
          Type = type' }

type ShaderProgram() as it =
    let mutable disposed = false
    let ShaderProgramID = GL.CreateProgram()
    let mutable VertexShaderID = 0
    let mutable FragmentShaderID = 0

    let mutable ShaderUniforms: Option<ShaderUniform[]> = Option.None
    let mutable ShaderAttributes: Option<ShaderAttribute[]> = Option.None

    do
        it.CompileVertexShader
        it.CompileFragmentShader

        it.CreateAndLinkProgram

        ShaderUniforms <- it.CreateUniformList |> Option.Some
        ShaderAttributes <- it.CreateAttributeList |> Option.Some

    member this.GetUniformList = Array.copy ShaderUniforms.Value

    member this.GetAttributeList = Array.copy ShaderAttributes.Value

    member this.SetUniform(name, v1: float32) =
        let uniform = ShaderUniforms.Value |> Array.find (fun u -> u.Name = name)
        match uniform.Type with
        | ActiveUniformType.Float -> do
                GL.UseProgram(ShaderProgramID)
                GL.Uniform1(uniform.Location, float32 v1)
                GL.UseProgram(0)
        | _ -> failwith "Unsupported"

    member this.SetUniform(name, v1: float, v2: float) =
        let uniform = ShaderUniforms.Value |> Array.find (fun u -> u.Name = name)
        match uniform.Type with
        | ActiveUniformType.FloatVec2 -> do
                GL.UseProgram(ShaderProgramID)
                GL.Uniform2(uniform.Location, float32 v1, float32 v2)
                GL.UseProgram(0)
        | _ -> failwith "Unsupported"

    member this.CompileVertexShader =
        let vertShaderPath = @"shaders\firstVertexShader.glsl"
        let vertShaderSource = File.ReadAllText(vertShaderPath)
        VertexShaderID <- GL.CreateShader(ShaderType.VertexShader)
        GL.ShaderSource(VertexShaderID, vertShaderSource)
        GL.CompileShader(VertexShaderID)

        let shaderInfo = GL.GetShaderInfoLog(VertexShaderID)
        if (shaderInfo <> "") then printfn "%s" shaderInfo

    member this.CompileFragmentShader =
        let fragShaderPath = @"shaders\firstFragmentShader.glsl"
        let fragShaderSource = File.ReadAllText(fragShaderPath)
        FragmentShaderID <- GL.CreateShader(ShaderType.FragmentShader)
        GL.ShaderSource(FragmentShaderID, fragShaderSource)
        GL.CompileShader(FragmentShaderID)

        let shaderInfo = GL.GetShaderInfoLog(FragmentShaderID)
        if (shaderInfo <> "") then printfn "%s" shaderInfo

    member this.CreateAndLinkProgram =
        GL.AttachShader(ShaderProgramID, VertexShaderID)
        GL.AttachShader(ShaderProgramID, FragmentShaderID)
        GL.LinkProgram(ShaderProgramID)
        GL.DetachShader(ShaderProgramID, VertexShaderID)
        GL.DetachShader(ShaderProgramID, FragmentShaderID)

    member this.CreateUniformList =
        let mutable uniformCount = 0
        GL.GetProgram(ShaderProgramID, GetProgramParameterName.ActiveUniforms, &uniformCount)
        [| for i in [0 .. uniformCount-1] ->
            let mutable type': ActiveUniformType = ActiveUniformType.Bool
            let mutable name = ""
            let mutable length = 0
            let mutable size = 0
            GL.GetActiveUniform(ShaderProgramID, i, 256, &length, &size, &type', &name)
            let location = GL.GetUniformLocation(ShaderProgramID, name)
            new ShaderUniform(name, location, type') |]

    member this.CreateAttributeList =
        let mutable attrCount = 0
        GL.GetProgram(ShaderProgramID, GetProgramParameterName.ActiveAttributes, &attrCount)
        [| for i in [0 .. attrCount-1] ->
            let mutable type': ActiveAttribType = ActiveAttribType.None
            let mutable name = ""
            let mutable length = 0
            let mutable size = 0
            GL.GetActiveAttrib(ShaderProgramID, i, 256, &length, &size, &type', &name)
            new ShaderAttribute(name, GL.GetAttribLocation(ShaderProgramID, name), type') |]

    member this.ID = ShaderProgramID

    member this.Cleanup =
        if not disposed then
            GL.UseProgram(0)
            GL.DeleteShader(VertexShaderID)
            GL.DeleteShader(FragmentShaderID)
            GL.UseProgram(0)
            GL.DeleteProgram(ShaderProgramID)
            disposed <- true

    interface IDisposable with
        member it.Dispose() =
            it.Cleanup
            GC.SuppressFinalize(it)

    override this.Finalize() = it.Cleanup
