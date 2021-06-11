/*
 * Castaway Built-In Shader `default-tex`
 * Vertex Shader
 */

#version 150 core
// GLSL 1.50 (OpenGL 3.2)

in vec3 vPosition;
in vec3 vColor;
in vec3 vNormal;
in vec2 vTextureCoords;

out vec3 fColor;
out vec2 fTextureCoords;
out vec3 fNormal;
out vec3 fFragmentPosition;

uniform mat4 tPersp;
uniform mat4 tView;
uniform mat4 tModel;

void main() {
    gl_Position = tPersp * tView * tModel * vec4(vPosition, 1);
    fColor = vColor;
    fTextureCoords = vTextureCoords;
    fNormal = vNormal;
    fFragmentPosition = vec3(tModel * vec4(vPosition, 1));
}
