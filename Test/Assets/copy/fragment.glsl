#version 400 core

in vec2 texCoord;
out vec4 outCol;

uniform sampler2D tex;

void main() {
    outCol = texture(tex, texCoord);
}