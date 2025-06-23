# apr 과제를 위한 프로젝트 입니다.

# 환경설정
- .NET 9.0 이상 필요

# 프로젝트 빌드
### Windows (PowerShell)
```
cd {PROJECT_DIR}
dotnet.exe build
```
### MacOS
```
cd {PROJECT_DIR}
dotnet build
```

# 실행
### Windows (PowerShell)
```
dotnet.exe run {PROJECT_DIR}/ImageService
# ex> dotnet.exe run "C:\Users\USER\Documents\apr\ImageService"
```
### MacOS
```
dotnet run {PROJECT_DIR}/ImageService
```

# 참고사항
- *jpg, *jpeg, *png 파일 변환 가능
- 파일에 띄어쓰기가 있을 시 파일을 읽지 못함
