#version 150 core

in vec3 inPosition;
in vec4 inColor;

out vec4 color;

uniform mat4 model;
uniform mat4 view;
uniform mat4 proj;

void main()
{
    gl_Position = proj * view * model * vec4(inPosition, 1);
    color = inColor;
}
