#version 150 core

in vec3 inPosition;
in vec4 inColor;
in vec3 inNormal;
in vec2 inTexCoords;

out vec4 color;
out vec3 normal;
out vec3 fragPos;
out vec2 texCoords;

uniform mat4 model;
uniform mat4 view;
uniform mat4 proj;

mat4 lastModel = mat4(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
mat4 tiModel = mat4(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);

void main()
{
    if(model != lastModel) {
        tiModel = transpose(inverse(model));
        lastModel = model;
    }
    gl_Position = proj * view * model * vec4(inPosition, 1);
    color = inColor;
    normal = mat3(tiModel) * inNormal;
    texCoords = inTexCoords;
    fragPos = vec3(model * vec4(inPosition, 1));
}
