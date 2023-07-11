#version 330 core

uniform vec2 viewportSize;
uniform float colorFactor;

layout (location = 0) in vec2 aPos;
layout (location = 1) in vec4 aColor;

out vec4 vertexColor;

void main()
{
    float nx = (aPos.x / viewportSize.x) * 2.0f - 1.0f;
    float ny = (aPos.y / viewportSize.y) * 2.0f - 1.0f;

    gl_Position = vec4(nx, ny, 0.0, 1.0);

    vertexColor = aColor * colorFactor;
}