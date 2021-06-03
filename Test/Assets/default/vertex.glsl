#version 400 core

in vec3 inPos;
in vec4 inCol;
in vec2 inTex;

out vec4 col;
out vec2 texCoord;

uniform mat4 transform;

void main() {
    gl_Position = transform * vec4(inPos, 1);
    col = inCol;
    texCoord = inTex;
}
