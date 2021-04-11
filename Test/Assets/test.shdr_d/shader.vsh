#version 150 core

in vec3 inPosition;
in vec4 inColor;

out vec4 color;

void main()
{
    gl_Position = vec4(inPosition, 1);
    color = inColor;
}
