/*
 * Castaway Built-In Shader `notransform`
 * Vertex Shader
 */

// USES FRAGMENT FROM `default`!

#version 150 core
// GLSL 1.50 (OpenGL 3.2)

in vec3 vPosition;
in vec3 vColor;
in vec2 vTextureCoords;

out vec3 fColor;
out vec2 fTextureCoords;

void main() {
    gl_Position = vec4(vPosition, 1);
    fColor = vColor;
    fTextureCoords = vTextureCoords;
}
