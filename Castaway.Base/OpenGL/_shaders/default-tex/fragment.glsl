/*
 * Castaway Built-In Shader `default-tex`
 * Fragment Shader
 */

#version 150 core
// GLSL 1.50 (OpenGL 3.2)

in vec3 fColor;
in vec2 fTextureCoords;

out vec4 oColor;

uniform sampler2D uTexture;

void main() {
    oColor = texture(uTexture, fTextureCoords) * vec4(fColor, 1);
}
