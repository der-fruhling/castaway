#version 460 core

in vec2 pos;
in vec3 inCol;

out vec4 col;

uniform mat4 transform;

void main() {
    gl_Position = transform * vec4(pos, 0, 1);
    col = vec4(inCol, 1);
}
