/*
 * Castaway Built-In Shader `default-tex`
 * Vertex Shader
 */

#version 150 core
// GLSL 1.50 (OpenGL 3.2)

in vec3 vPosition;
in vec3 vColor;
in vec2 vTextureCoords;

out vec3 fColor;
out vec2 fTextureCoords;

uniform mat4 tPersp;
uniform mat4 tView;
uniform mat4 tModel;

void main() {
    gl_Position = tPersp * tView * tModel * vec4(vPosition, 1);
    fColor = vColor;
    fTextureCoords = vTextureCoords;
}
