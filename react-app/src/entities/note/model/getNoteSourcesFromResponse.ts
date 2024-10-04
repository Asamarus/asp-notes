import type { NoteSource } from './types'
import type { components } from '@/shared/api'

function getNoteSourcesFromResponse(
  sources: components['schemas']['SourceItemResponse'][],
): NoteSource[] {
  return (
    sources?.map((source) => ({
      id: source.id ?? '',
      title: source.title ?? '',
      link: source.link ?? '',
      description: source.description ?? '',
      image: source.image ?? '',
      showImage: source.showImage ?? false,
    })) ?? []
  )
}

export default getNoteSourcesFromResponse
