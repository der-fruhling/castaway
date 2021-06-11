/*
 * Castaway Built-In Shader `notransform-tex`
 * Fragment Shader
 */

#version 150 core
// GLSL 1.50 (OpenGL 3.2)

in vec3 fColor;
in vec2 fTextureCoords;

out vec4 oColor;

uniform sampler2D tex;

void main() {
    oColor = texture(tex, fTextureCoords) * vec4(fColor, 1);
}
