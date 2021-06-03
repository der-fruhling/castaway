#version 400 core

in vec2 texCoord;
out vec4 outCol;

uniform sampler2D tex;

vec4 blur(vec2 pos) {
    float xPix = 1f / 800f;
    float yPix = 1f / 600f;
    
    vec3 sum = vec3(0, 0, 0);
    int count = 0;
    
    const int size = 9;
    
    for(int i = -size; i <= size; i++)
        for(int j = -size; j <= size; j++) {
            sum += vec3(texture(tex, pos + vec2(float(i) * xPix, float(j) * yPix)));
            count++;
        }
    
    return vec4(sum / float(count), 1);
}

void main() {
    outCol = blur(texCoord);
}