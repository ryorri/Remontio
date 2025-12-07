<template>
  <div>
    <!-- Custom value calculation (Type 4) -->
    custorm
  </div>
</template>

<script lang="ts" setup>
import { ref } from 'vue'

defineProps<{
  roomId: string
  submitting: boolean
}>()

const emit = defineEmits<{
  back: []
  submit: [data: { calculationName: string; value: number }]
}>()
</script>

<style scoped></style>
