#version 400 core

in vec4 col;
in vec2 texCoord;

out vec4 outCol;

uniform sampler2D tex1;
uniform sampler2D tex2;
uniform float intensity;

void main() {
    vec4 cat1 = texture(tex1, texCoord);
    vec4 cat2 = texture(tex2, texCoord);
    outCol = mix(cat1, cat2, intensity) * col;
}