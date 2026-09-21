import sys, os, re
sys.path.insert(0, r'D:\person_data\workfold\projects\RHI_zh_cn')
from translate import translate_string

fp = r'D:\person_data\workfold\projects\RHI_zh_cn\RenoDXCommander\MainWindow.xaml'
with open(fp, 'r', encoding='utf-8') as f:
    content = f.read()

# isolate SettingsPanel region (line 1095 onward)
lines = content.split('\n')
settings_text = '\n'.join(lines[1094:])

pattern = re.compile(r'''(\w[\w.:]*)="([^"]*)"''')
seen = set()
out = []
skip_attrs = {'xmlns', 'x:Name', 'x:Class', 'x:Key', 'Key', 'Tag', 'Uid', 'Name',
              'x:Bind', 'x:DeferLoadStrategy', 'x:Uid', 'DefaultStyleKey',
              'x:Phase', 'x:FieldModifier'}
for m in pattern.finditer(settings_text):
    attr, val = m.group(1), m.group(2)
    if attr in skip_attrs or attr.startswith('xmlns'):
        continue
    if not val or val.startswith('{') or val.startswith('/'):
        continue
    if not any('a' <= c <= 'z' or 'A' <= c <= 'Z' for c in val):
        continue
    if val in seen:
        continue
    seen.add(val)
    if translate_string(val) != val:
        continue  # already translated
    out.append(f'{attr}="{val}"')

with open(r'D:\person_data\workfold\projects\RHI_zh_cn\settings_xaml.txt', 'w', encoding='utf-8') as f:
    f.write('\n'.join(out))
print(f'{len(out)} untranslated settings XAML strings')
