import 'bootstrap/dist/css/bootstrap.css'
import 'bootstrap/dist/js/bootstrap.bundle.min.js'
import './assets/styles/main.css'
import { FontAwesomeIcon } from './assets/styles/fortawesome'
import { createApp } from 'vue'
import { createPinia } from 'pinia'
import App from './App.vue'
import router from './router'

// Backend
import { RemontioBackend } from './backend/RemontioBackend'
const backend = new RemontioBackend('https://localhost:7259')

const app = createApp(App)

app.use(createPinia())
app.use(router)

app.component('font-awesome-icon', FontAwesomeIcon)
app.config.globalProperties.$apiClient = backend

app.mount('#app')

export { backend as Backend }
