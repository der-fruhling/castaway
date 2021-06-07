/*
 * Castaway Built-In Shader `default`
 * Vertex Shader
 */

#version 150 core
// GLSL 1.50 (OpenGL 3.2)

in vec3 vPosition;
in vec3 vColor;

out vec3 fColor;

uniform mat4 tPersp;
uniform mat4 tView;
uniform mat4 tModel;

void main() {
    gl_Position = tPersp * tView * tModel * vec4(vPosition, 1);
    fColor = vColor;
}
