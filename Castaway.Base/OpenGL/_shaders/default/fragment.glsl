/*
 * Castaway Built-In Shader `default`
 * Fragment Shader
 */

#version 150 core
// GLSL 1.50 (OpenGL 3.2)

in vec3 fColor;

out vec4 oColor;

void main() {
    oColor = vec4(fColor, 1);
}
