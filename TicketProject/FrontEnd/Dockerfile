# 使用 node 的官方映像作為基礎映像
FROM node:18

# 設置工作目錄
WORKDIR /app

# 複製 package.json 和 package-lock.json 以利用 Docker 的緩存機制
COPY package*.json ./

# 安裝依賴並輸出日誌
RUN if [ -f package.json ]; then npm install && echo "NPM install completed"; fi

# 複製其餘的應用程式碼
COPY . .

# 暴露應用的端口
EXPOSE 3000

# 設定運行命令
CMD ["npm", "run", "dev"]