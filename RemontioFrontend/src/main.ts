import 'bootstrap/dist/css/bootstrap.css'
import './assets/styles/main.css'
import { createApp } from 'vue'
import { createPinia } from 'pinia'

import App from './App.vue'
import router from './router'

import { Client } from './backend/BackendBase'
const backend = new Client('https://localhost:7259')

const app = createApp(App)

app.use(createPinia())
app.use(router)

app.config.globalProperties.$apiClient = backend

app.mount('#app')
export { backend as Backend }