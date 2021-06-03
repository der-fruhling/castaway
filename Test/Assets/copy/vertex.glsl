#version 400 core

in vec2 inPos;
in vec2 inTex;

out vec2 texCoord;

void main() {
    gl_Position = vec4(inPos, 0, 1);
    texCoord = inTex;
}
